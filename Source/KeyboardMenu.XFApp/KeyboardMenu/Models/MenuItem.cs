using System;
using System.Collections.Generic;
using KeyboardMenu.Interfaces;

namespace KeyboardMenu.Models
{
    public class MenuItem : ISelectableItem
    {
        public int Order { get; set; }
        public string IconFile { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Value => String.Empty;
        public IList<SubMenuItem> SubMenuItems { get; set; }

        public static IList<MenuItem> GetMenuItems()
        {
            var result = new List<MenuItem>();

            Tuple<string, string[]>[] menuItems = {
                Tuple.Create("Home", new[] { "Address", "Application", "Book", "Calendar", "Camera", "Devices", "Gear" }),
                Tuple.Create("Business", new[] { "Agreement", "Briefcase", "Calculator", "Card", "Contract", "Delivery", "Graph" }),
                Tuple.Create("Security", new[] { "Certificate", "Code", "Justice", "Login", "Malware", "Padlock", "Shield" }),
                Tuple.Create("Hardware", new[] { "Chip", "CPU", "Fan", "Printer", "Router", "SSD", "Webcam" }),
            };

            for (int i = 0; i < menuItems.Length; i++)
            {
                string menuName = menuItems[i].Item1;
                var subitems = new List<SubMenuItem>();
                for (int j = 0; j < menuItems[i].Item2.Length; j++)
                {
                    string subMenuName = menuItems[i].Item2[j];
                    var choices = new List<ChoiceItem>();
                    for (int k = 1; k < 6; k++)
                    {
                        choices.Add(new ChoiceItem
                        {
                            Order = k,
                            IconFile = $"{menuName.ToLower()}-{subMenuName.ToLower()}-{k}.svg"
                        });
                    }
                    subitems.Add(new SubMenuItem
                    {
                        Order = j,
                        Name = subMenuName,
                        IconFile = $"{menuName.ToLower()}-{subMenuName.ToLower()}.svg",
                        ChoiceItems = choices,
                        IsActive = (j == 0)
                    });
                }
                result.Add(new MenuItem
                {
                    Order = i,
                    Name = menuName,
                    IconFile = $"{menuName.ToLower()}.svg",
                    SubMenuItems = subitems,
                    IsActive = (i == 0)
                });
            }

            return result;
        }
    }
}
