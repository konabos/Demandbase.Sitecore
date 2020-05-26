using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules;
using Sitecore.Rules.ConditionalRenderings;
using System;
using System.Linq;
using System.Text;

namespace SitecoreDemandbase
{
    public class PersonalizationContext
    {
        public static string GenerateMetaTags()
        {
            StringBuilder sb = new StringBuilder();
            RuleStack stack = new RuleStack();
            var references = Sitecore.Context.Item.Visualization.GetRenderings(Sitecore.Context.Device, false);
            try
            {
                foreach (RenderingReference reference in references)
                {
                    ConditionalRenderingsRuleContext context = new ConditionalRenderingsRuleContext(references.ToList(), reference);
                    foreach (Rule<ConditionalRenderingsRuleContext> rule in reference.Settings.Rules.Rules)
                    {
                        rule.Condition.Evaluate(context, stack);
                        if ((stack.Count != 0) && (bool)stack.Pop() && rule.UniqueId != ID.Null)
                        {
                            sb.Append(",").Append(rule.Name);
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warn("Problem aggregating rules for personalization context, metatag may be incorrect", e, e);
            }
            return sb.Length > 0 ? $@"<meta name=""sitecorePersonalization"" content=""{sb.ToString(1, sb.Length - 1)}"">" : "";
        }
    }
}
