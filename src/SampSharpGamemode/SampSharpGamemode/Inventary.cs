﻿using System;
using System.Collections.Generic;
using System.Text;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Definitions;

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
        public Item[] items = new Item[_MAX_ITEMS];

        public Inventary() { }

        public void Set(Item item, int pos)
        {
            items[pos] = item;
        }
        public Item Get(int pos)
        {
            return items[pos];
        }
        public static void Show(Player player)
        {
            var inv_d = new ListDialog("Инвентарь", "Выбор", "Закрыть");
            for (int i = 0; i < MAX_ITEMS; i++)
                inv_d.AddItem(player.inventary.Get(i).name);

            inv_d.Response += (sender, e) =>
            {
                if(e.DialogButton == DialogButton.Left)
                {
                    player.SendClientMessage("В разработке...");
                }
            };

            inv_d.Show(player);
        }
    }
}
