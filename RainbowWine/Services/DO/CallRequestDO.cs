using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DO
{
    public class CallRequestDO
    {
        public string callFlowId { get; set; } = "TUMspyjWoYb+Ul8vp2khpgWZix3lECvaXcJtTQ78KKLBPo36QQvadUqNeDy1V80Y7FezqpU+11BtNjQiEhzRIg==";
        public string customerId { get; set; } = "Quosphere";
        public string callType { get; set; } = "OUTBOUND";
        public CallFlowConfiguration callFlowConfiguration { get; set; }
        public CallRequestDO()
        {
            callFlowConfiguration = new CallFlowConfiguration();
        }

    }
    public class CallFlowConfiguration
    {
        public InitiateCall_1 initiateCall_1 { get; set; }
        public InitiateCall_1 addParticipant_1 { get; set; }
        public Record record { get; set; }

        public CallFlowConfiguration()
        {
            initiateCall_1 = new InitiateCall_1()
            {
                callerId = "8045810885",
                mergingStrategy = "SEQUENTIAL",
                maxTime = 0,
                participants = new List<Participant>()
                {
                    new Participant()
                    {
                      callerId="8045810885",
                      maxRetries=1,
                      maxTime=0,
                      participantAddress="9881634394",
                      participantName="DELIVERY BOY"
                    }
                },
                callBackURLs = new List<CallBackURL>()
                 {
                     new CallBackURL()
                     {
                          eventType="CDR",
                           method="POST",
                            notifyURL=string.Empty
                     },
                     new CallBackURL()
                     {
                          eventType="ALL",
                           method="POST",
                            notifyURL=string.Empty
                     }
                 }
            };
            addParticipant_1 = new InitiateCall_1()
            {
                mergingStrategy = "SEQUENTIAL",
                maxTime = 0,
                participants = new List<Participant>()
                 {
                     new Participant()
                     {
                         callerId="8045810885",
                         enableEarlyMedia=true,
                          maxTime=0,
                          maxRetries=1,
                          participantAddress="7498263131",
                           participantName="B"
                     }
                 }
            };

            record = new Record();
        }
    }

    public class InitiateCall_1
    {
        public string callerId { get; set; }
        public string mergingStrategy { get; set; }
        public int maxTime { get; set; }
        public List<CallBackURL> callBackURLs { get; set; }
        public List<Participant> participants { get; set; }
    }

    public class Record
    {
        public bool enabled { get; set; } = true;
    }

    public class Participant
    {
        public string callerId { get; set; }
        public int maxRetries { get; set; }
        public int maxTime { get; set; }
        public string participantAddress { get; set; }
        public string participantName { get; set; }
        public bool enableEarlyMedia { get; set; }
    }

    public class CallBackURL
    {
        public string eventType { get; set; }
        public string method { get; set; }
        public string notifyURL { get; set; }

    }
}