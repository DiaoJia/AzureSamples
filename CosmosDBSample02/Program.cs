using CosmosDBSample02.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDBSample02
{
    class Program
    {
        private static List<Item> _items = new List<Item>();
        static void Main(string[] args)
        {
            DocumentDBRepository<Item>.Initialize();
            GetItemsAsync().Wait();
            foreach (var item in _items)
            {
                Console.WriteLine($"ItemId = {item.Id},ItemCategory = {item.Category}");
            }

            Console.WriteLine("Complete!");
            Console.Read();
        }

        private static async Task GetItemsAsync()
        {
            var items = await DocumentDBRepository<Item>.GetItemsAsync(d => !d.Completed);
            _items = items.ToList();
        }
    }
}
