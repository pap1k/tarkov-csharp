using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharpGamemode
{
    public class Item
    {
        public int id,
                   type,
                   max_stack,
                   amount;

        public string name,
                      image;

        public bool takeable,
                    droppable,
                    stackable;
        public Item(int id, int type, string name, string image, bool takeable, bool droppable, bool stackable, int max_stack)
        {
            this.id = id;
            this.type = type;
            this.max_stack = max_stack;
            this.name = name;
            this.image = image;
            this.takeable = takeable;
            this.droppable = droppable;
            this.stackable = stackable;
        }

    }
    public class Inventary
    {
        private const int _MAX_ITEMS = 36;
        public static int MAX_ITEMS{ get => _MAX_ITEMS; }
        public Item[] items;

        public Inventary(int[] item_ids, int[] stacks)
        {
            items = new Item[_MAX_ITEMS];
            for(int i = 0; i < _MAX_ITEMS; i++)
            {
                items[i].id = item_ids[i];
                items[i].amount = stacks[i];
            }
        }
    }

}
