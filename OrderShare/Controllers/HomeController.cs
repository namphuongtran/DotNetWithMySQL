using OrderShare.Common;
using OrderShare.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace OrderShare.Controllers
{
    public class HomeController : Controller
    {
        orderEntities orderDB = new orderEntities();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ReadData()
        {
            bool isSuccess = false;
            string message = string.Empty;
            try
            {
                string filePath = ConfigurationManager.AppSettings["FilePath"];
                if (!string.IsNullOrEmpty(filePath))
                {
                    bool isExistFile = System.IO.File.Exists(filePath);
                    if (isExistFile)
                    {
                        string fileExtension = System.IO.Path.GetExtension(filePath);
                        if (fileExtension == ".html")
                        {
                            string contentFile = System.IO.File.ReadAllText(filePath);
                            if (!string.IsNullOrEmpty(contentFile))
                            {
                                string tablePattern = "<table\\b[^>]*?>(?<T>[\\s\\S]*?)</\\s*table>";
                                MatchCollection tableMatCol = Regex.Matches(contentFile, tablePattern, RegexOptions.IgnoreCase);
                                int i = 0;
                                foreach (Match match in tableMatCol)
                                {
                                    string tableContent = match.Groups["T"].Value;
                                    if (!string.IsNullOrEmpty(tableContent))
                                    {
                                        switch (i)
                                        {
                                            case 0:
                                                //get account table
                                                account account = GetAccountInfo(tableContent);
                                                orderDB.accounts.Add(account);
                                                orderDB.SaveChanges();
                                                break;
                                            case 1:
                                                //get open table
                                                List<openorder> openOrders = GetOpenOrder(tableContent);
                                                orderDB.openorders.AddRange(openOrders);
                                                orderDB.SaveChanges();
                                                break;
                                            case 2:
                                                //get pending table
                                                List<pendingorder> pendingOrders = GetPendingOrder(tableContent);
                                                orderDB.pendingorders.AddRange(pendingOrders);
                                                orderDB.SaveChanges();
                                                break;
                                            case 3:
                                                //get close table
                                                List<closeorder> closeOrders = GetCloseOrder(tableContent);
                                                orderDB.closeorders.AddRange(closeOrders);
                                                orderDB.SaveChanges();
                                                break;
                                            default:
                                                break;
                                        }
                                        isSuccess = true;
                                    }
                                    else
                                    {
                                        isSuccess = false;
                                        message = "Table content is empty";
                                    }
                                    i++;
                                }
                            }
                            else
                            {
                                isSuccess = false;
                                message = "File content is empty";
                            }
                        }
                        else
                        {
                            isSuccess = false;
                            message = "Only HTML files are supported";
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = "File Does Not Exist";
                    }
                }
                else
                {
                    isSuccess = false;
                    message = "File Path Does Not Empty in WebConfig";
                }
            }
            catch (Exception ex)
            {
                message = "There's an error occurred while loading data. Please try again later. " + ex.Message;
                isSuccess = false;
            }
            return Json(new { IsSuccess = isSuccess, Message = isSuccess ? "Insert data successfully" : message }, JsonRequestBehavior.AllowGet);
        }

        private account GetAccountInfo(string accountTable)
        {
            account acc = new account();
            if (!string.IsNullOrEmpty(accountTable))
            {
                string tdPattern = "<td\\b[^>]*?>(?<V>[\\s\\S]*?)</\\s*td>";
                MatchCollection mc = Regex.Matches(accountTable, tdPattern, RegexOptions.IgnoreCase);
                int i = 0;
                foreach (Match match in mc)
                {
                    string value = match.Groups["V"].Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        int startIndexB = value.IndexOf("<b>");
                        int endIndexB = value.IndexOf("</b>");
                        int labelIndex = endIndexB + "</b>".Length + startIndexB;
                        string normalVal = value.Substring(endIndexB + "</b>".Length, value.Length - labelIndex);
                        switch (i)
                        {
                            case 0:
                                acc.AccountNo = normalVal;
                                break;
                            case 1:
                                acc.Name = normalVal;
                                break;
                            case 2:
                                acc.Broker = normalVal;
                                break;
                            case 3:
                            case 7:
                            case 9:
                                break;
                            case 4:
                                acc.Equity = Convert.ToDecimal(normalVal);
                                break;
                            case 5:
                                acc.FreeMargin = Convert.ToDecimal(normalVal);
                                break;
                            case 6:
                                acc.Currency = normalVal;
                                break;
                            case 8:
                                int startDateIndex = normalVal.IndexOf("[");
                                int endDateIndex = normalVal.IndexOf("]");
                                string timeZone = normalVal.Substring(startDateIndex, normalVal.Length - startDateIndex);
                                string updateTime = normalVal.Replace(timeZone, string.Empty);
                                acc.UpdateTime = Convert.ToDateTime(updateTime.Trim());
                                break;
                            default:
                                break;
                        }
                    }
                    i++;
                }
            }
            return acc;
        }

        private List<openorder> GetOpenOrder(string openTable)
        {
            List<openorder> openOrders = new List<openorder>();
            if (!string.IsNullOrEmpty(openTable))
            {
                string trPattern = "<tr\\b[^>]*?>(?<R>[\\s\\S]*?)</\\s*tr>";
                MatchCollection trMatCol = Regex.Matches(openTable, trPattern, RegexOptions.IgnoreCase);
                int i = 0;
                foreach (Match trMatch in trMatCol)
                {
                    string row = trMatch.Groups["R"].Value;
                    if (!string.IsNullOrEmpty(row))
                    {
                        if (i > 1)
                        {
                            string tdPattern = "<td\\b[^>]*?>(?<C>[\\s\\S]*?)</\\s*td>";
                            MatchCollection tdMatCol = Regex.Matches(row, tdPattern, RegexOptions.IgnoreCase);
                            int j = 0;
                            openorder openOrder = new openorder();
                            foreach (Match tdMach in tdMatCol)
                            {
                                string col = tdMach.Groups["C"].Value.Trim();
                                if (!string.IsNullOrEmpty(col))
                                {
                                    switch (j)
                                    {
                                        case 0:
                                            openOrder.Ticket = col;
                                            break;
                                        case 1:
                                            openOrder.Symbol = col;
                                            break;
                                        case 2:
                                            openOrder.Type = col;
                                            break;
                                        case 3:
                                            openOrder.Size = float.Parse(col);
                                            break;
                                        case 4:
                                            openOrder.OpenTime = Convert.ToDateTime(col);
                                            break;
                                        case 5:
                                            openOrder.OpenPrice = Convert.ToDecimal(col);
                                            break;
                                        case 6:
                                            openOrder.SL = Convert.ToDecimal(col);
                                            break;
                                        case 7:
                                            openOrder.TP = Convert.ToDecimal(col);
                                            break;
                                        case 8:
                                            openOrder.Swap = float.Parse(col);
                                            break;
                                        case 9:
                                            openOrder.Profit = float.Parse(col);
                                            break;
                                        case 10:
                                            openOrder.PipsValue = float.Parse(col);
                                            break;
                                        case 11:
                                            openOrder.Comment = col;
                                            break;
                                        default:
                                            break;
                                    }

                                }
                                j++;
                            }
                            openOrders.Add(openOrder);
                        }
                    }
                    i++;
                }
            }
            return openOrders;
        }

        private List<pendingorder> GetPendingOrder(string pendingTable)
        {
            List<pendingorder> pendingOrders = new List<pendingorder>();
            if (!string.IsNullOrEmpty(pendingTable))
            {
                string trPattern = "<tr\\b[^>]*?>(?<R>[\\s\\S]*?)</\\s*tr>";
                MatchCollection trMatCol = Regex.Matches(pendingTable, trPattern, RegexOptions.IgnoreCase);
                int i = 0;
                foreach (Match trMatch in trMatCol)
                {
                    string row = trMatch.Groups["R"].Value;
                    if (!string.IsNullOrEmpty(row))
                    {
                        if (i > 1)
                        {
                            string tdPattern = "<td\\b[^>]*?>(?<C>[\\s\\S]*?)</\\s*td>";
                            MatchCollection tdMatCol = Regex.Matches(row, tdPattern, RegexOptions.IgnoreCase);
                            int j = 0;
                            pendingorder pendingOrder = new pendingorder();
                            foreach (Match tdMach in tdMatCol)
                            {
                                string col = tdMach.Groups["C"].Value.Trim();
                                if (!string.IsNullOrEmpty(col))
                                {
                                    switch (j)
                                    {
                                        case 0:
                                            pendingOrder.Ticket = col;
                                            break;
                                        case 1:
                                            pendingOrder.Symbol = col;
                                            break;
                                        case 2:
                                            pendingOrder.Type = col;
                                            break;
                                        case 3:
                                            pendingOrder.Size = float.Parse(col);
                                            break;
                                        case 4:
                                            pendingOrder.OpenTime = Convert.ToDateTime(col);
                                            break;
                                        case 5:
                                            pendingOrder.OpenPrice = Convert.ToDecimal(col);
                                            break;
                                        case 6:
                                            pendingOrder.SL = Convert.ToDecimal(col);
                                            break;
                                        case 7:
                                            pendingOrder.TP = Convert.ToDecimal(col);
                                            break;
                                        case 8:
                                            pendingOrder.ExpireTime = Convert.ToDateTime(col);
                                            break;
                                        case 9:
                                            pendingOrder.PipsValue = float.Parse(col);
                                            break;
                                        case 10:
                                            pendingOrder.Comment = col;
                                            break;
                                        default:
                                            break;
                                    }

                                }
                                j++;
                            }
                            pendingOrders.Add(pendingOrder);
                        }
                    }
                    i++;
                }
            }
            return pendingOrders;
        }

        private List<closeorder> GetCloseOrder(string closeTable)
        {
            List<closeorder> closeOrders = new List<closeorder>();
            if (!string.IsNullOrEmpty(closeTable))
            {
                string trPattern = "<tr\\b[^>]*?>(?<R>[\\s\\S]*?)</\\s*tr>";
                MatchCollection trMatCol = Regex.Matches(closeTable, trPattern, RegexOptions.IgnoreCase);
                int i = 0;
                foreach (Match trMatch in trMatCol)
                {
                    string row = trMatch.Groups["R"].Value;
                    if (!string.IsNullOrEmpty(row))
                    {
                        if (i > 1)
                        {
                            string tdPattern = "<td\\b[^>]*?>(?<C>[\\s\\S]*?)</\\s*td>";
                            MatchCollection tdMatCol = Regex.Matches(row, tdPattern, RegexOptions.IgnoreCase);
                            int j = 0;
                            closeorder closeOrder = new closeorder();
                            foreach (Match tdMach in tdMatCol)
                            {
                                string col = tdMach.Groups["C"].Value.Trim();
                                if (!string.IsNullOrEmpty(col))
                                {
                                    switch (j)
                                    {
                                        case 0:
                                            closeOrder.Ticket = col;
                                            break;
                                        case 1:
                                            closeOrder.Symbol = col;
                                            break;
                                        case 2:
                                            closeOrder.Type = col;
                                            break;
                                        case 3:
                                            closeOrder.Size = float.Parse(col);
                                            break;
                                        case 4:
                                            closeOrder.OpenTime = Convert.ToDateTime(col);
                                            break;
                                        case 5:
                                            closeOrder.OpenPrice = Convert.ToDecimal(col);
                                            break;
                                        case 6:
                                            closeOrder.SL = Convert.ToDecimal(col);
                                            break;
                                        case 7:
                                            closeOrder.TP = Convert.ToDecimal(col);
                                            break;
                                        case 8:
                                            closeOrder.CloseTime = Convert.ToDateTime(col);
                                            break;
                                        case 9:
                                            closeOrder.Swap = float.Parse(col);
                                            break;
                                        case 10:
                                            closeOrder.Profit = float.Parse(col);
                                            break;
                                        case 11:
                                            closeOrder.PipsValue = float.Parse(col);
                                            break;
                                        case 12:
                                            closeOrder.Comment = col;
                                            break;
                                        default:
                                            break;
                                    }

                                }
                                j++;
                            }
                            closeOrders.Add(closeOrder);
                        }
                    }
                    i++;
                }
            }
            return closeOrders;
        }
    }
}