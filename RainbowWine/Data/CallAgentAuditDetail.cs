//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RainbowWine.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class CallAgentAuditDetail
    {
        public int Id { get; set; }
        public int AuditID { get; set; }
        public string CustomerContact { get; set; }
        public int OrderID { get; set; }
        public string VOC { get; set; }
        public string CallType { get; set; }
        public string CallOpening { get; set; }
        public string CallClosing { get; set; }
        public string DeadAirHold { get; set; }
        public string ClarityOfSpeech { get; set; }
        public string ChoiceOfWords { get; set; }
        public string ActiveListening { get; set; }
        public string Tone { get; set; }
        public string Ownership { get; set; }
        public string Information { get; set; }
        public string AppropriateAction { get; set; }
        public string Documentation { get; set; }
        public string Ztp { get; set; }
        public string Strengths { get; set; }
        public string AreasOfImprovement { get; set; }
        public string OverallStatus { get; set; }
    
        public virtual CallAuditMaster CallAuditMaster { get; set; }
    }
}
