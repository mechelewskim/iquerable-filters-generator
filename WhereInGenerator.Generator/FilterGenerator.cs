using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;

namespace WhereInGenerator.Generator
{
    [Generator]
    public class FilterGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<FilterToGenerate> enumsToGenerate = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                    transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                .Where(static m => m is not null);

            context.RegisterSourceOutput(enumsToGenerate,
                static (spc, source) => Execute(source, spc));
        }

        static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        {
            return node is PropertyDeclarationSyntax m
                && m.AttributeLists.Count > 0
                && m.AttributeLists.Any(x => x.Attributes.Any(y => y.ToString() == "GenerateWhereFilter"));
        }

        static FilterToGenerate GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            var property = (PropertyDeclarationSyntax)context.Node;

            var parentClass = (ClassDeclarationSyntax)property.Parent;

            var namespaceName = (NamespaceDeclarationSyntax) parentClass.Parent;

            return new FilterToGenerate(parentClass.Identifier.ToString(), property.Identifier.ToString(), property.Type.ToString(), namespaceName.Name.ToString());
        }

        static void Execute(FilterToGenerate enumToGenerate, SourceProductionContext context)
        {
            string result =
                $@"
                    using System.Linq;
                    using { enumToGenerate.ClassNamespace };

                    namespace Company.Generated;

                    public static class {enumToGenerate.GetUniqueIdentifier()}FilterExtensions
                    {{
                        public static IQueryable<{enumToGenerate.ClassName}> Where{enumToGenerate.PropertyName}In(this IQueryable<{enumToGenerate.ClassName}> query, {enumToGenerate.PropertyType}[] value)
                        {{
                            if (value == null || value.Length == 0)
                            {{
                                return query;
                            }}

                            { GenerateCasesFor(1, 50, enumToGenerate.PropertyName) }

                            return query.Where(x => value.Contains(x.{enumToGenerate.PropertyName}));
                        }}
                    }}
                ";

            context.AddSource($"{enumToGenerate.ClassName}Extensions.Where{enumToGenerate.PropertyName}In.g.cs", SourceText.From(result, Encoding.UTF8));
        }

        private static string GenerateCasesFor(int minCount, int maxCount, string propertyName)
        {
            var stringBuilder = new StringBuilder();

            for (int i = minCount; i <= maxCount; i++)
            {
                var conditions = string.Join(" || ", Enumerable.Range(0, i).Select(x => $"x.{propertyName} == value[{x}]"));

                stringBuilder.AppendLine($@"
                    if (value.Length == {i})
                    {{
                        return query.Where(x => {conditions});
                    }}");
            }

            return stringBuilder.ToString();
        }
    }
}
