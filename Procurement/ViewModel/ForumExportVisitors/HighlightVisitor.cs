using System.Collections.Generic;
using POEApi.Model;
using Procurement.ViewModel.Filters;
using System.Linq;
using Procurement.ViewModel.Filters.ForumExport;

namespace Procurement.ViewModel.ForumExportVisitors
{
    internal class HighlightVisitor : VisitorBase
    {
        private const string TOKEN = "{Highlights}";
        public override string Visit(IEnumerable<Item> items, string current)
        {
            return current.Replace(TOKEN, runFilter<HighlightFilter>(items));
        }
    }
}