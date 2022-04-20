using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Google.Cloud.Firestore;
using static RainbowWine.Services.FireStoreService;

namespace RainbowWine.Services
{
    public class FireStoreService
    {
        [FirestoreData]
        public class OrderFireStore
        {
            [FirestoreProperty]
            public int OrderId { get; set; }
            [FirestoreProperty]
            public string OrderStatus { get; set; }
            [FirestoreProperty]
            public int ShopID { get; set; }
            [FirestoreProperty]
            public GeoPoint ShopLoc { get; set; }
            [FirestoreProperty]
            public ArrayList StatusData { get; set; }
            [FirestoreProperty]
            public int CustomerId { get; set; }
            [FirestoreProperty]
            public GeoPoint CustomerLoc { get; set; }
            [FirestoreProperty]
            public int AgentId { get; set; }
            [FirestoreProperty]
            public GeoPoint AgentLoc { get; set; }
            [FirestoreProperty]
            public Timestamp Timestamp { get; set; }
            [FirestoreProperty]
            public bool CompletedFlag { get; set; }
            [FirestoreProperty]
            public bool isOrderStart { get; set; }
            [FirestoreProperty]
            public int OrderAmount { get; set; }
            [FirestoreProperty]
            public string AgentContactNo { get; set; }

            [FirestoreProperty]
            public int PaymentTypeId { get; set; }
            [FirestoreProperty]
            public int OrderStatusId { get; set; }

            [FirestoreProperty]
            public bool IsFirebaseCustAppOn { get; set; } = false;

            [FirestoreProperty]
            public bool IsHyperTrackOn { get; set; } = false;

            [FirestoreProperty]
            public string HyperTrackDeviceId { get; set; } = string.Empty;

            [FirestoreProperty]
            public string TripId { get; set; } = string.Empty;

            [FirestoreProperty]
            public bool CanTrackAgentLoc { get; set; } = false;

            [FirestoreProperty]
            public bool ShowPrebookConfetti { get; set; } = false;

            [FirestoreProperty]
            public string PrebookConfettiTitle { get; set; } = string.Empty;

            [FirestoreProperty]
            public string PrebookConfettiBody { get; set; } = string.Empty;

            [FirestoreProperty]
            public bool IsScheduledOrder { get; set; } = false;

            [FirestoreProperty]
            public string TotalItemQty { get; set; }

            [FirestoreProperty]
            public string SchDeliveryText { get; set; }

            

        }
    }
    public class StatusChange
    {
        public string DisplayStatus { get; set; }
        public string Icon { get; set; }
        public string Time { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsActive { get; set; }
      
    }

    public class HookDetails
    {
        public int HookID { get; set; }
        public string Title { get; set; }
        public string SubText { get; set; }
        public string Icon { get; set; }
        public string Payload { get; set; }
        public string BgImage { get; set; }
        public object BgColors { get; set; }
        public bool NeedCartClear { get; set; }


    }
    public class FireStoreAccess
    {
        string projectId;
        FirestoreDb fireStoreDb;
        public FireStoreAccess()
        {
            string filepath = "C:\\FirestoreAPIKey\\supple-cubist-141812-1bc94f943ad9.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            projectId = "supple-cubist-141812";
            fireStoreDb = FirestoreDb.Create(projectId);
        }

        public void AddToFireStore(OrderFireStore orderFireStore, List<StatusChange> statusTrack, HookDetails hookDetails=null)
        {
            try
            {
                var doc_name = "Order_" + orderFireStore.OrderId.ToString();
                DocumentReference colRef = fireStoreDb.Collection("Orders").Document(doc_name);
                int i = 0;

                ArrayList arrayExample = new ArrayList();
                foreach (var statusItem in statusTrack)
                {
                    if (i == 0 && hookDetails !=null)
                    {
                        var hookItem = hookDetails.GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                      .ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(hookDetails, null));
                        arrayExample.Add(hookItem);

                    }
                    
                   var dictItem =  statusItem.GetType()
                 .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                      .ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(statusItem, null));
                    arrayExample.Add(dictItem);
                    i++;
                }
                
                orderFireStore.StatusData = arrayExample;
                colRef.SetAsync(orderFireStore);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public void DeleteDocfromFireStore(int orderId)
        {
            try
            {
                var doc_name = "Order_" + orderId.ToString();
                DocumentReference cityRef = fireStoreDb.Collection("Orders").Document(doc_name);
                 cityRef.DeleteAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UpdateFireStore(int orderId,string agentContactNo)
        {
            try
            {
                var doc_name = "Order_" + orderId.ToString();
                DocumentReference cityRef = fireStoreDb.Collection("Orders").Document(doc_name);
                cityRef.UpdateAsync("AgentContactNo",agentContactNo);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}