﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainbowWine.Models
{
    public enum ConfigEnums
    {
        MinLimitOrder = 1,
        MinLimitOrderMsg = 2,
        UPIEnable = 3,
        PODEnable = 4,
        MaxiumPodAmount = 5,
        LiveSite = 6,
        MixerEnable = 7,
        GiftCharge = 8,
        IsMixerPOD =9,
        DeliveryCharges=10,
        NumberOfOrder = 11 ,
        SZCreditsCanUse =12 ,
        ExpireInDays =13 ,
        IsWalletForPOD = 14 ,
        CanWeUSeWalletForOrder = 16,
        IsSignWithoutCode = 17 ,
        Base60                =18 ,          
        Base90                = 19,
        ExtraDistEarning60    = 20,
        ExtraDistEarning90    = 21,
        FixedDistance         = 22,
        ShopsDistanceRule     =23 ,
        DBPosReviewAmt        = 24,
        DBNegReviewAmt        = 25,
        Penalty_75minsAbove   = 26,
        DeliveryReachedDistCheckFlag = 27,
        MinDistDeliveryReached= 28,
        AirtelIQUsername      = 29,
        AirtelIQPassword      = 30,
        CustScoreThreshold    = 31,
        TrackToFirestore      = 35 ,
        IpRestriction = 36 ,
        IsPODTempFixed =37 ,
        PODEnablev2 = 38,
        LoginDistCheck = 39 ,
        CashBackApplicable =40,
        Incentive = 41 ,
        RatingPenaltyFlag =42 ,
        IncPenaltyFlag = 43 ,
        StarPenaltyAmt = 44,
        IsCachingApplied = 45,
        SlotStartWOLogin =46 ,
        VerifyPrepaidOTPFlag = 47,
        VerifyPostpaidOTPFlag = 48,
        VerifyPostpaidUrl = 49,
        VerifyPrepaidUrl = 50 ,
        FirebaseTrack = 51,
        FirebaseInterval = 52 ,
        GAPIKey_iOS      =53 ,
        GAPIKey_Android  = 54,
        ForcedUpdateApp_iOS = 55,
        ForcedUpdateApp_Android =56,
        LatestVersion_iOS = 57,
        LatestVersion_Android = 58 ,
        IsEnableDistanceMatrix = 59 ,
        DistanceMatrixInterval = 60 ,
        GAPIKeyDistance_iOS =61 ,
        IsHyperTrackOn = 62,
        HyperTrackBasicAuthToken =63 ,
        SlotStartedStatusList = 64 ,
        HyperTrackPubKey      = 65 ,
        IsFirebaseCustAppOn   = 66 ,
        SlotStartWithoutHandover = 67 ,
        isDelAppHyperTrackOn = 68 ,
        VersionText = 69 ,
        HyperTrackSecret = 70 ,
        PaymentGateWayName =71 ,
        ServiceableShopChFlag = 72,
        ServiceableZoneChFlag = 73 ,
        NotifyDeliveryManager = 74 ,
        ReferAndEarnHook = 105 ,
        WebEngageEnable = 107 ,
        PODSucessStatus = 111 ,
        TrackDelAgent = 112,
        TrackAllDelAgent = 113,
        ThresholdPenaltyCount = 114,
        PenaltyWarningTitle = 115,
        PenaltyWarningBody = 116,
        PenaltyTitle = 117,
        PenaltyBody = 118 ,
        IsPenalyApply =120 ,
        IsReturnCouponAmount = 121,
        AssignAPICall =122
    }
}