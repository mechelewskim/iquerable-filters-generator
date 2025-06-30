namespace WhereInGenerator.Generator
{
    internal class FilterToGenerate
    {
        public string ClassName;
        public string ClassNamespace;
        public string PropertyName;
        public string PropertyType;

        public FilterToGenerate(string className, string propertyName, string propertyType, string classNamespace)
        {
            ClassName = className;
            PropertyName = propertyName;
            PropertyType = propertyType;
            ClassNamespace = classNamespace;
        }

        public string GetUniqueIdentifier() => $"{ClassName}{PropertyName}";
    }
}
