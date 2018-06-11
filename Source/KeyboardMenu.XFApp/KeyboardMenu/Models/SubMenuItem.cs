using System;
using System.Collections.Generic;
using KeyboardMenu.Interfaces;

namespace KeyboardMenu.Models
{
    public class SubMenuItem : ISelectableItem
    {
        public int Order { get; set; }
        public string IconFile { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Value => String.Empty;
        public IList<ChoiceItem> ChoiceItems { get; set; }
    }
}
