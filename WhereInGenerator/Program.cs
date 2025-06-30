using System;
using System.Collections.Generic;
using System.Linq;
using WhereInGenerator;
using Company.Generated;

Console.WriteLine("Hello, World!");

var list = new List<Entity> 
{ 
    new Entity { Id = 1, Name = "Test-1", AccountId = 10 },
    new Entity { Id = 2, Name = "Test-2", AccountId = 20 },
    new Entity { Id = 3, Name = "Test-3", AccountId = 30 }
};

Console.WriteLine("==BEFORE FILTERING==");

foreach (var item in list)
{
    Console.WriteLine($"Id: {item.Id}, Name: {item.Name}, AccountId: {item.AccountId}");
}

var query = list.AsQueryable();

query = query.WhereAccountIdIn([20]);

Console.WriteLine("==AFTER FILTERING==");

foreach (var item in query.ToList())
{
    Console.WriteLine($"Id: {item.Id}, Name: {item.Name}, AccountId: {item.AccountId}");
}
