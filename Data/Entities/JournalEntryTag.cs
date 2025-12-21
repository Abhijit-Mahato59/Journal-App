using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalApp.Data.Entities;

public class JournalEntryTag
{
    public int JournalEntryId { get; set; }
    public JournalEntry JournalEntry { get; set; } = default!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = default!;
}
