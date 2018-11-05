using System;
using System.Collections.Immutable;

namespace SideTopics
{
    public class Container
    {
        public int ID { get; }

        public string Description { get; }

        public DateTime ProductionDate { get; }

        public Container SubContainer { get; }

        public Container(int id, string description, DateTime productionDate, Container subContainer)
        {
            ID = id;
            Description = description;
            ProductionDate = productionDate;
            SubContainer = subContainer;
        }

        public Container SetDescription(string newDescription) =>
            new Container(ID, newDescription, ProductionDate, SubContainer);
    }

    public static class ImmutableCollections
    {
        public static void ShowImmutables()
        {
            // Read more about immutable collections at
            // https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable?view=netcore-2.1#remarks

            var containers = ImmutableList<Container>.Empty;
            containers = containers.Add(new Container(1, "Foo", DateTime.Today, null));
            containers = containers.Add(new Container(1, "Bar", DateTime.Today, null));
            foreach(var container in containers)
            {
                Console.WriteLine(container.Description);
            }
        }
    }
}
