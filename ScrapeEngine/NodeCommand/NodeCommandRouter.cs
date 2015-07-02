using HtmlAgilityPack;
using ScrapeEngine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScrapeEngine.NodeCommand
{
    public class NodeCommandRouter
    {
        internal static Dictionary<string, string> RouteNode(HtmlNode node, List<NodeCommandObject> CommandCollection)
        {
            Dictionary<string, string> obj = new Dictionary<string, string>();
            foreach (NodeCommandObject nodeCmd in CommandCollection)
            {
                switch (nodeCmd.Commands.Count)
                {
                    case 1: obj.Add(nodeCmd.DomainObjectProperty, SingleNodeCommand.SingleProperty(node, nodeCmd));
                        break;
                    case 2: obj.Add(nodeCmd.DomainObjectProperty, DualNodeCommand.DualProperty(node, nodeCmd));
                        break;
                }
            }

            return obj;
        }
    }

    public static class SingleNodeCommand
    {
        internal static string SingleProperty(HtmlNode node, NodeCommandObject cmd)
        {
            string value = string.Empty;
            MethodInfo info = node.GetType().GetMethod(cmd.Commands[0].Operation);
            object[] param = new object[] { cmd.Xpath };

            if (cmd.Commands[0].Property.Equals("Attribute"))
            {
                HtmlNode _nodeFromRoot = (HtmlNode)info.Invoke(node, param);
                value = _nodeFromRoot.Attributes[cmd.Commands[0].Value].Value;
            }
            else
            {
                try
                {
                    HtmlNode _nodeFromRoot = (HtmlNode)info.Invoke(node, param);
                    value = (string)_nodeFromRoot.GetType().GetProperty(cmd.Commands[0].Property).GetValue(_nodeFromRoot, null);
                }
                catch
                {

                }
            }
            return value;
        }
    }

    public static class DualNodeCommand
    {
        internal static string DualProperty(HtmlNode node, NodeCommandObject cmd)
        {
            string value = string.Empty;
            MethodInfo info = node.GetType().GetMethod(cmd.Commands[0].Operation);
            object[] param = new object[] { cmd.Xpath };
            try
            {
                HtmlNode _nodeFromRoot = (HtmlNode)info.Invoke(node, param);
                var initial = _nodeFromRoot.GetType().GetProperty(cmd.Commands[0].Property).GetValue(_nodeFromRoot);
                value = (string)initial.GetType().GetProperty(cmd.Commands[1].Property).GetValue(initial);
            }
            catch
            {

            }

            return value;
        }
    }
}
