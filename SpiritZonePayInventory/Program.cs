using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SpiritZonePayInventory
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Date : " + DateTime.Now);
            //Console.ReadKey();
            //hooktesing();
            var cronType = ConfigurationManager.AppSettings["CronType"];
            if (cronType == "1")
                LoadRefundInitiatedOrders();
            else
                LoadOrders();
        }

        private static void LoadOrders()
        {
            PaytmOrder paytmOrder = new PaytmOrder();
            List<int> orders = new List<int>();
            var dt = paytmOrder.GetOrderListCashFree();
            foreach (DataRow row in dt.Rows)
            {
                var orderId = Convert.ToInt32(row["Id"]);
                var mobileno = Convert.ToString(row["OrderTo"]);

                if (!orders.Contains(orderId))
                {
                    orders.Add(orderId);
                    paytmOrder.UpdateInventory(orderId);

                    SendSMS sendSMS = new SendSMS();
                    string txt = string.Format(ConfigurationManager.AppSettings["SMSCancel"], orderId);
                    sendSMS.SendMessage(txt, mobileno);
                }
            }
        }

        private static void LoadOrdersOld()
        {
            PaytmOrder paytmOrder = new PaytmOrder();
            List<int> orders = new List<int>();
            var dt = paytmOrder.GetOrderList();
            foreach (DataRow row in dt.Rows)
            {
                var orderId = Convert.ToInt32(row["orderId"]);
                var mobileno = Convert.ToString(row["OrderTo"]);
                Console.WriteLine("row[PayUrlExpiry] : " + row["PayUrlExpiry"]);

                var convertDateFor = DateTime.ParseExact(row["PayUrlExpiry"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                var expDate = Convert.ToDateTime(convertDateFor);
                Console.WriteLine("after conversion");

                var curdate = DateTime.Now;
                if (expDate.Year >= 2018)
                {
                    if (curdate > expDate)
                    {
                        if (!orders.Contains(orderId))
                        {
                            orders.Add(orderId);
                            paytmOrder.UpdateInventory(orderId);

                            SendSMS sendSMS = new SendSMS();
                            string txt = string.Format(ConfigurationManager.AppSettings["SMSCancel"], orderId);
                            sendSMS.SendMessage(txt, mobileno);
                        }
                    }
                }
            }
        }
        private static void LoadRefundInitiatedOrders()
        {
            PaytmOrder paytmOrder = new PaytmOrder();
            List<int> orders = new List<int>();
            var dt = paytmOrder.GetPaytmRefundInitiatedList();
            foreach (DataRow row in dt.Rows)
            {
                var orderId = Convert.ToInt32(row["orderId"]);
                var payRefundId = Convert.ToInt32(row["PaymentRefundId"]);
                if (!orders.Contains(orderId))
                {
                    orders.Add(orderId);
                    using (var client = new HttpClient())
                    {
                        Uri url = new Uri(ConfigurationManager.AppSettings["RefundStatusUrl"]);
                        StringContent stringContent = new StringContent("{ \"orderId\": " + orderId + " }", Encoding.UTF8, "application/json");
                        var response = client.PostAsync(url, stringContent).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var ret = response.Content.ReadAsStringAsync().Result;
                        }
                    }
                }
            }
        }

        private static void hooktesing()
        {
            PaytmOrder paytmOrder = new PaytmOrder();
            var dt = paytmOrder.GettestingList();
            foreach (DataRow row in dt.Rows)
            {
                var inputPaytm = Convert.ToString(row["InputPaytm"]);
                var id = Convert.ToInt32(row["AppLogsPaytmHookId"]);

                var objdata = HttpUtility.ParseQueryString(inputPaytm);
                var decodevlaue = new PaytmSetApproveResponse
                {
                    CHECKSUMHASH = objdata["CHECKSUMHASH"],
                    LINKNOTES = objdata["LINKNOTES"],
                    MERC_UNQ_REF = objdata["MERC_UNQ_REF"],
                    STATUS = objdata["STATUS"],
                    TXNAMOUNT = objdata["TXNAMOUNT"],
                    ORDERID = objdata["ORDERID"],
                    TXNID = objdata["TXNID"],
                    RESPMSG = objdata["RESPMSG"]
                };
                var ptmLinkId = decodevlaue.MERC_UNQ_REF.Replace("LI_", "");
                paytmOrder.updattestingList(id, ptmLinkId, decodevlaue.ORDERID, decodevlaue.STATUS, decodevlaue.CHECKSUMHASH, 
                    decodevlaue.TXNAMOUNT, decodevlaue.TXNID, decodevlaue.LINKNOTES, decodevlaue.RESPMSG);

            }

        }
    }

    public class PaytmSetApproveResponse
    {
        public string STATUS { get; set; }
        public string CHECKSUMHASH { get; set; }
        public string TXNAMOUNT { get; set; }
        public string ORDERID { get; set; }
        public string MERC_UNQ_REF { get; set; }
        public string LINKNOTES { get; set; }
        public string TXNID { get; set; }
        public string RESPMSG { get; set; }

    }
}


