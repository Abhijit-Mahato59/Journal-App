using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace JournalApp.Data.Entities;

public class Tag
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = "";

    public bool IsPredefined { get; set; }

    public List<JournalEntryTag> JournalEntryTags { get; set; } = new();
}
