
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 06/19/2020 22:03:45
-- Generated from EDMX file: E:\Qonverse\Project\SpiritZone\RainbowWine\RainbowWine\Data\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [RainbowwineModel];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_CustomerAddress_Customer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CustomerAddress] DROP CONSTRAINT [FK_CustomerAddress_Customer];
GO
IF OBJECT_ID(N'[dbo].[FK_CustomerAddress_WineShop]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CustomerAddress] DROP CONSTRAINT [FK_CustomerAddress_WineShop];
GO
IF OBJECT_ID(N'[dbo].[FK_CustomerOrder]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [FK_CustomerOrder];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserClaims] DROP CONSTRAINT [FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserLogins] DROP CONSTRAINT [FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserRoles_dbo_AspNetRoles_RoleId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_dbo_AspNetUserRoles_dbo_AspNetRoles_RoleId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserRoles_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_dbo_AspNetUserRoles_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_DeliveryAgentLogin_DeliveryAgents]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DeliveryAgentLogin] DROP CONSTRAINT [FK_DeliveryAgentLogin_DeliveryAgents];
GO
IF OBJECT_ID(N'[dbo].[FK_DeliveryAgents_DeliverySlots]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DeliveryAgents] DROP CONSTRAINT [FK_DeliveryAgents_DeliverySlots];
GO
IF OBJECT_ID(N'[dbo].[FK_DeliveryAgents_DeliveryZones]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DeliveryAgents] DROP CONSTRAINT [FK_DeliveryAgents_DeliveryZones];
GO
IF OBJECT_ID(N'[dbo].[FK_DeliveryAgents_WineShop]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DeliveryAgents] DROP CONSTRAINT [FK_DeliveryAgents_WineShop];
GO
IF OBJECT_ID(N'[dbo].[FK_DeliveryZones_WineShop]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DeliveryZones] DROP CONSTRAINT [FK_DeliveryZones_WineShop];
GO
IF OBJECT_ID(N'[dbo].[FK_DumpRoutePlan_Customer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DumpRoutePlan] DROP CONSTRAINT [FK_DumpRoutePlan_Customer];
GO
IF OBJECT_ID(N'[dbo].[FK_DumpRoutePlan_DeliveryAgents]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DumpRoutePlan] DROP CONSTRAINT [FK_DumpRoutePlan_DeliveryAgents];
GO
IF OBJECT_ID(N'[dbo].[FK_DumpRoutePlan_Orders]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DumpRoutePlan] DROP CONSTRAINT [FK_DumpRoutePlan_Orders];
GO
IF OBJECT_ID(N'[dbo].[FK_DumpRoutePlan_OrderStatus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DumpRoutePlan] DROP CONSTRAINT [FK_DumpRoutePlan_OrderStatus];
GO
IF OBJECT_ID(N'[dbo].[FK_DumpRoutePlan_WineShop]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DumpRoutePlan] DROP CONSTRAINT [FK_DumpRoutePlan_WineShop];
GO
IF OBJECT_ID(N'[dbo].[FK_Inventory_ProductDetails]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Inventory] DROP CONSTRAINT [FK_Inventory_ProductDetails];
GO
IF OBJECT_ID(N'[dbo].[FK_Inventory_WineShop]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Inventory] DROP CONSTRAINT [FK_Inventory_WineShop];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderDetail_Orders]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderDetail] DROP CONSTRAINT [FK_OrderDetail_Orders];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderDetail_ProductDetails1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderDetail] DROP CONSTRAINT [FK_OrderDetail_ProductDetails1];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderDetail_WineShop]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderDetail] DROP CONSTRAINT [FK_OrderDetail_WineShop];
GO
IF OBJECT_ID(N'[dbo].[FK_Orders_OrderStatus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [FK_Orders_OrderStatus];
GO
IF OBJECT_ID(N'[dbo].[FK_Orders_WineShop]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [FK_Orders_WineShop];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderTrack_OrderStatus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderTrack] DROP CONSTRAINT [FK_OrderTrack_OrderStatus];
GO
IF OBJECT_ID(N'[dbo].[FK_PayLink_Orders]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PayLink] DROP CONSTRAINT [FK_PayLink_Orders];
GO
IF OBJECT_ID(N'[dbo].[FK_Product_ProductBrand]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Product] DROP CONSTRAINT [FK_Product_ProductBrand];
GO
IF OBJECT_ID(N'[dbo].[FK_Product_ProductCategory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Product] DROP CONSTRAINT [FK_Product_ProductCategory];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductDetails_Product]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProductDetails] DROP CONSTRAINT [FK_ProductDetails_Product];
GO
IF OBJECT_ID(N'[dbo].[FK_RoutePlan_Customer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoutePlan] DROP CONSTRAINT [FK_RoutePlan_Customer];
GO
IF OBJECT_ID(N'[dbo].[FK_RoutePlan_DeliveryAgents]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoutePlan] DROP CONSTRAINT [FK_RoutePlan_DeliveryAgents];
GO
IF OBJECT_ID(N'[dbo].[FK_RoutePlan_Orders]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoutePlan] DROP CONSTRAINT [FK_RoutePlan_Orders];
GO
IF OBJECT_ID(N'[dbo].[FK_RoutePlan_OrderStatus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoutePlan] DROP CONSTRAINT [FK_RoutePlan_OrderStatus];
GO
IF OBJECT_ID(N'[dbo].[FK_RoutePlan_WineShop]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoutePlan] DROP CONSTRAINT [FK_RoutePlan_WineShop];
GO
IF OBJECT_ID(N'[dbo].[FK_RUser_AspNetUsers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RUser] DROP CONSTRAINT [FK_RUser_AspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[FK_RUser_DeliveryAgents]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RUser] DROP CONSTRAINT [FK_RUser_DeliveryAgents];
GO
IF OBJECT_ID(N'[dbo].[FK_RUser_UserType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RUser] DROP CONSTRAINT [FK_RUser_UserType];
GO
IF OBJECT_ID(N'[dbo].[FK_RUser_WineShop]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RUser] DROP CONSTRAINT [FK_RUser_WineShop];
GO
IF OBJECT_ID(N'[dbo].[FK_ShopMerchant_MerchantTrans]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ShopMerchant] DROP CONSTRAINT [FK_ShopMerchant_MerchantTrans];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AppLogs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AppLogs];
GO
IF OBJECT_ID(N'[dbo].[AppLogsCashFreeHook]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AppLogsCashFreeHook];
GO
IF OBJECT_ID(N'[dbo].[AppLogsPaytmHook]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AppLogsPaytmHook];
GO
IF OBJECT_ID(N'[dbo].[AspNetRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetRoles];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserClaims]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserClaims];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserLogins]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserLogins];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserRoles];
GO
IF OBJECT_ID(N'[dbo].[AspNetUsers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[CallbackPushApi]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CallbackPushApi];
GO
IF OBJECT_ID(N'[dbo].[Customer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Customer];
GO
IF OBJECT_ID(N'[dbo].[CustomerAddress]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CustomerAddress];
GO
IF OBJECT_ID(N'[dbo].[CustomerContact]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CustomerContact];
GO
IF OBJECT_ID(N'[dbo].[CustomerOTPVerify]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CustomerOTPVerify];
GO
IF OBJECT_ID(N'[dbo].[DeliveryAgentLogin]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeliveryAgentLogin];
GO
IF OBJECT_ID(N'[dbo].[DeliveryAgents]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeliveryAgents];
GO
IF OBJECT_ID(N'[dbo].[DeliveryAgentTime]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeliveryAgentTime];
GO
IF OBJECT_ID(N'[dbo].[DeliveryAgentTrack]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeliveryAgentTrack];
GO
IF OBJECT_ID(N'[dbo].[DeliveryHours]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeliveryHours];
GO
IF OBJECT_ID(N'[dbo].[DeliveryJob]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeliveryJob];
GO
IF OBJECT_ID(N'[dbo].[DeliverySlots]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeliverySlots];
GO
IF OBJECT_ID(N'[dbo].[DeliveryZones]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeliveryZones];
GO
IF OBJECT_ID(N'[dbo].[DumpRoutePlan]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DumpRoutePlan];
GO
IF OBJECT_ID(N'[dbo].[Inventory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Inventory];
GO
IF OBJECT_ID(N'[dbo].[MerchantTrans]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MerchantTrans];
GO
IF OBJECT_ID(N'[dbo].[OrderDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderDetail];
GO
IF OBJECT_ID(N'[dbo].[Orders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Orders];
GO
IF OBJECT_ID(N'[dbo].[OrderStatus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderStatus];
GO
IF OBJECT_ID(N'[dbo].[OrderTrack]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderTrack];
GO
IF OBJECT_ID(N'[dbo].[PayLink]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PayLink];
GO
IF OBJECT_ID(N'[dbo].[PaymentCashFreeLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PaymentCashFreeLog];
GO
IF OBJECT_ID(N'[dbo].[PaymentLinkLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PaymentLinkLog];
GO
IF OBJECT_ID(N'[dbo].[PaymentRefund]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PaymentRefund];
GO
IF OBJECT_ID(N'[dbo].[Product]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Product];
GO
IF OBJECT_ID(N'[dbo].[ProductBrand]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductBrand];
GO
IF OBJECT_ID(N'[dbo].[ProductCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductCategory];
GO
IF OBJECT_ID(N'[dbo].[ProductCategoryLink]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductCategoryLink];
GO
IF OBJECT_ID(N'[dbo].[ProductDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductDetails];
GO
IF OBJECT_ID(N'[dbo].[ProductLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductLog];
GO
IF OBJECT_ID(N'[dbo].[ProductNotThere]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductNotThere];
GO
IF OBJECT_ID(N'[dbo].[ProductRecom]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductRecom];
GO
IF OBJECT_ID(N'[dbo].[ProductSize]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductSize];
GO
IF OBJECT_ID(N'[dbo].[RoutePlan]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RoutePlan];
GO
IF OBJECT_ID(N'[dbo].[RUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RUser];
GO
IF OBJECT_ID(N'[dbo].[ShopMerchant]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ShopMerchant];
GO
IF OBJECT_ID(N'[dbo].[TravelMode]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TravelMode];
GO
IF OBJECT_ID(N'[dbo].[UserRefreshToken]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserRefreshToken];
GO
IF OBJECT_ID(N'[dbo].[UserType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserType];
GO
IF OBJECT_ID(N'[dbo].[WineShop]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WineShop];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AppLogs'
CREATE TABLE [dbo].[AppLogs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CreateDatetime] datetime  NOT NULL,
    [Message] nvarchar(max)  NULL,
    [Error] nvarchar(max)  NULL,
    [MachineName] nvarchar(255)  NULL
);
GO

-- Creating table 'AppLogsCashFreeHooks'
CREATE TABLE [dbo].[AppLogsCashFreeHooks] (
    [AppLogsCashFreeHookId] int IDENTITY(1,1) NOT NULL,
    [VenderInput] nvarchar(max)  NULL,
    [CreatedDate] datetime  NULL,
    [OrderId] nvarchar(200)  NULL,
    [OrderAmount] nvarchar(20)  NULL,
    [ReferenceId] nvarchar(255)  NULL,
    [Status] nvarchar(20)  NULL,
    [PaymentMode] nvarchar(20)  NULL,
    [Msg] nvarchar(max)  NULL,
    [TxtTime] nvarchar(30)  NULL,
    [Signature] nvarchar(max)  NULL,
    [SendStatus] nvarchar(50)  NULL,
    [MachineName] nvarchar(100)  NULL
);
GO

-- Creating table 'AppLogsPaytmHooks'
CREATE TABLE [dbo].[AppLogsPaytmHooks] (
    [AppLogsPaytmHookId] int IDENTITY(1,1) NOT NULL,
    [InputPaytm] nvarchar(max)  NULL,
    [PtmOrderId] nvarchar(255)  NULL,
    [LinkOrderId] int  NULL,
    [OrderId] int  NULL,
    [CreateDate] datetime  NULL,
    [PtmStatus] nvarchar(50)  NULL,
    [SendStatus] nvarchar(50)  NULL,
    [PtmLinkId] int  NULL,
    [TxnId] nvarchar(255)  NULL,
    [TxnAmount] nvarchar(20)  NULL,
    [PtmRespMsg] nvarchar(255)  NULL,
    [LinkNotes] nvarchar(200)  NULL,
    [CheckSumHash] nvarchar(max)  NULL,
    [MachineName] nvarchar(100)  NULL
);
GO

-- Creating table 'AspNetRoles'
CREATE TABLE [dbo].[AspNetRoles] (
    [Id] nvarchar(128)  NOT NULL,
    [Name] nvarchar(256)  NOT NULL
);
GO

-- Creating table 'AspNetUserClaims'
CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] nvarchar(128)  NOT NULL,
    [ClaimType] nvarchar(max)  NULL,
    [ClaimValue] nvarchar(max)  NULL
);
GO

-- Creating table 'AspNetUserLogins'
CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider] nvarchar(128)  NOT NULL,
    [ProviderKey] nvarchar(128)  NOT NULL,
    [UserId] nvarchar(128)  NOT NULL
);
GO

-- Creating table 'AspNetUsers'
CREATE TABLE [dbo].[AspNetUsers] (
    [Id] nvarchar(128)  NOT NULL,
    [Email] nvarchar(256)  NULL,
    [EmailConfirmed] bit  NOT NULL,
    [PasswordHash] nvarchar(max)  NULL,
    [SecurityStamp] nvarchar(max)  NULL,
    [PhoneNumber] nvarchar(max)  NULL,
    [PhoneNumberConfirmed] bit  NOT NULL,
    [TwoFactorEnabled] bit  NOT NULL,
    [LockoutEndDateUtc] datetime  NULL,
    [LockoutEnabled] bit  NOT NULL,
    [AccessFailedCount] int  NOT NULL,
    [UserName] nvarchar(256)  NOT NULL
);
GO

-- Creating table 'CallbackPushApis'
CREATE TABLE [dbo].[CallbackPushApis] (
    [CallbackPushApiId] int IDENTITY(1,1) NOT NULL,
    [customer_number] nvarchar(20)  NULL,
    [uuid] nvarchar(200)  NULL,
    [agent_number] nvarchar(200)  NULL,
    [call_duration] nvarchar(200)  NULL,
    [id] int  NULL,
    [call_recording] nvarchar(255)  NULL,
    [knowlarity_number] nvarchar(20)  NULL,
    [call_time] nvarchar(20)  NULL,
    [call_date] datetime  NULL,
    [call_status] nvarchar(200)  NULL,
    [call_transfer_status] nvarchar(200)  NULL,
    [conversation_duration] nvarchar(20)  NULL
);
GO

-- Creating table 'Customers'
CREATE TABLE [dbo].[Customers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] nvarchar(128)  NULL,
    [CreatedDate] datetime  NULL,
    [CustomerName] nvarchar(255)  NULL,
    [ContactNo] nvarchar(50)  NULL,
    [DOB] datetime  NULL,
    [Address] nvarchar(max)  NULL,
    [FormattedAddress] nvarchar(max)  NULL,
    [PlaceId] nvarchar(max)  NULL,
    [Flat] nvarchar(255)  NULL,
    [Landmark] nvarchar(255)  NULL,
    [ShopId] int  NULL,
    [RegisterSource] nvarchar(2)  NULL
);
GO

-- Creating table 'CustomerAddresses'
CREATE TABLE [dbo].[CustomerAddresses] (
    [CustomerAddressId] int IDENTITY(1,1) NOT NULL,
    [CustomerId] int  NOT NULL,
    [CreatedDate] datetime  NULL,
    [AddressType] nvarchar(20)  NULL,
    [Address] nvarchar(max)  NOT NULL,
    [FormattedAddress] nvarchar(max)  NULL,
    [Longitude] float  NULL,
    [Latitude] float  NULL,
    [PlaceId] nvarchar(max)  NULL,
    [Flat] nvarchar(255)  NULL,
    [Landmark] nvarchar(255)  NULL,
    [ShopId] int  NOT NULL
);
GO

-- Creating table 'CustomerContacts'
CREATE TABLE [dbo].[CustomerContacts] (
    [id] int IDENTITY(1,1) NOT NULL,
    [CustomerName] nvarchar(128)  NULL,
    [CustomerContact1] nvarchar(128)  NOT NULL,
    [CustAddr] nvarchar(max)  NULL,
    [CustPlaceId] nvarchar(128)  NOT NULL,
    [Latitude] decimal(12,9)  NULL,
    [Longitude] decimal(12,9)  NULL,
    [CallerAddress] nvarchar(max)  NULL
);
GO

-- Creating table 'CustomerOTPVerifies'
CREATE TABLE [dbo].[CustomerOTPVerifies] (
    [CustomerOTPVerifyId] int IDENTITY(1,1) NOT NULL,
    [OTP] varchar(50)  NULL,
    [CustomerId] int  NULL,
    [ContactNo] nvarchar(50)  NULL,
    [VerifiedDate] datetime  NULL,
    [CreatedDate] datetime  NULL,
    [IsDeleted] bit  NULL
);
GO

-- Creating table 'DeliveryAgentLogins'
CREATE TABLE [dbo].[DeliveryAgentLogins] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DeliveryAgentId] int  NOT NULL,
    [OnDuty] datetime  NOT NULL,
    [OffDuty] datetime  NULL,
    [IsOnOff] bit  NULL
);
GO

-- Creating table 'DeliveryAgents'
CREATE TABLE [dbo].[DeliveryAgents] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DeliveryExecName] nvarchar(max)  NOT NULL,
    [ShopID] int  NOT NULL,
    [LastDeliveryOn] datetime  NULL,
    [Contact] nvarchar(50)  NULL,
    [Address] nvarchar(max)  NULL,
    [TravelMode] nvarchar(50)  NULL,
    [Coverage] nvarchar(50)  NULL,
    [DeliverySlotID] int  NULL,
    [IsAtShop] bit  NULL,
    [ExciseID] nvarchar(128)  NULL,
    [DocPath] nvarchar(255)  NULL,
    [Present] bit  NULL,
    [ZoneId] int  NULL
);
GO

-- Creating table 'DeliveryAgentTimes'
CREATE TABLE [dbo].[DeliveryAgentTimes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DeliveryAgentId] int  NOT NULL,
    [DeliveryDate] datetime  NOT NULL,
    [RemainingMins] int  NULL
);
GO

-- Creating table 'DeliveryAgentTracks'
CREATE TABLE [dbo].[DeliveryAgentTracks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DeliveryAgentId] int  NULL,
    [OrderId] int  NULL,
    [StatusId] int  NULL,
    [TrackDate] datetime  NULL
);
GO

-- Creating table 'DeliveryHours'
CREATE TABLE [dbo].[DeliveryHours] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [StartTime] datetime  NOT NULL,
    [EndTime] datetime  NOT NULL,
    [isAllowed] bit  NOT NULL
);
GO

-- Creating table 'DeliveryJobs'
CREATE TABLE [dbo].[DeliveryJobs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [JobId] nvarchar(128)  NOT NULL,
    [DeliveryAgentId] int  NULL,
    [CompletedFlag] bit  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [IsReady] bit  NULL
);
GO

-- Creating table 'DeliverySlots'
CREATE TABLE [dbo].[DeliverySlots] (
    [DeliverySlotID] int  NOT NULL,
    [DeliverySlot_StartTime] datetime  NOT NULL,
    [DeliverySlot_EndTime] datetime  NOT NULL,
    [OrderDate_StartTime] datetime  NOT NULL,
    [OrderDate_EndTime] datetime  NOT NULL
);
GO

-- Creating table 'DeliveryZones'
CREATE TABLE [dbo].[DeliveryZones] (
    [ZoneID] int IDENTITY(1,1) NOT NULL,
    [ZoneName] nvarchar(128)  NULL,
    [ShopID] int  NULL,
    [OperationalFlag] bit  NULL,
    [PriceSlab] int  NULL
);
GO

-- Creating table 'DumpRoutePlans'
CREATE TABLE [dbo].[DumpRoutePlans] (
    [id] int IDENTITY(1,1) NOT NULL,
    [OrderID] int  NOT NULL,
    [ShopID] int  NOT NULL,
    [DestPlaceID] nvarchar(128)  NULL,
    [OrigPlaceID] nvarchar(128)  NULL,
    [CustID] int  NOT NULL,
    [RoutePlanLink] nvarchar(max)  NOT NULL,
    [OrderStatusId] int  NOT NULL,
    [DeliveryStart] datetime  NOT NULL,
    [DeliveryEnd] datetime  NOT NULL,
    [DeliveryAgentId] int  NULL,
    [AssignedDate] datetime  NULL,
    [ZoneID] int  NULL,
    [SlotStart] datetime  NULL,
    [SlotEnd] datetime  NULL,
    [JobId] nvarchar(128)  NULL,
    [isOutForDelivery] bit  NULL
);
GO

-- Creating table 'Inventories'
CREATE TABLE [dbo].[Inventories] (
    [InventoryId] int IDENTITY(1,1) NOT NULL,
    [ProductID] int  NOT NULL,
    [ShopID] int  NOT NULL,
    [QtyAvailable] int  NOT NULL,
    [MRP] float  NULL,
    [packing_size] int  NULL,
    [LastModified] datetime  NOT NULL,
    [LastModifiedBy] nvarchar(max)  NULL,
    [ShopItemId] nvarchar(50)  NULL
);
GO

-- Creating table 'MerchantTrans'
CREATE TABLE [dbo].[MerchantTrans] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MerchantName] nvarchar(255)  NULL,
    [MKey] nvarchar(255)  NULL,
    [MID] nvarchar(255)  NULL
);
GO

-- Creating table 'OrderDetails'
CREATE TABLE [dbo].[OrderDetails] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [OrderId] int  NOT NULL,
    [ProductID] int  NOT NULL,
    [ItemQty] int  NOT NULL,
    [Price] decimal(18,0)  NOT NULL,
    [ShopID] int  NOT NULL
);
GO

-- Creating table 'Orders'
CREATE TABLE [dbo].[Orders] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [OrderDate] datetime  NOT NULL,
    [OrderPlacedBy] nvarchar(max)  NULL,
    [OrderTo] nvarchar(50)  NULL,
    [OrderAmount] decimal(18,0)  NOT NULL,
    [CustomerId] int  NOT NULL,
    [CustomerAddressId] int  NULL,
    [OrderStatusId] int  NOT NULL,
    [LicPermitNo] nvarchar(100)  NULL,
    [ShopID] int  NULL,
    [ZoneId] int  NULL,
    [DeliveryDate] datetime  NULL,
    [DeliveryPickup] nvarchar(20)  NULL,
    [PaymentDevice] nvarchar(50)  NULL,
    [PaymentQRImage] nvarchar(max)  NULL,
    [PreOrder] smallint  NULL,
    [TestOrder] bit  NOT NULL,
    [NewOrderId] nvarchar(100)  NULL,
    [OrderType] varchar(2)  NULL
);
GO

-- Creating table 'OrderStatus'
CREATE TABLE [dbo].[OrderStatus] (
    [Id] int  NOT NULL,
    [OrderStatusName] nvarchar(15)  NULL
);
GO

-- Creating table 'OrderTracks'
CREATE TABLE [dbo].[OrderTracks] (
    [OrderTrackId] int IDENTITY(1,1) NOT NULL,
    [OrderId] int  NOT NULL,
    [TrackDate] datetime  NOT NULL,
    [StatusId] int  NOT NULL,
    [LogUserId] nvarchar(128)  NOT NULL,
    [LogUserName] nvarchar(255)  NOT NULL,
    [Remark] nvarchar(max)  NULL
);
GO

-- Creating table 'PayLinks'
CREATE TABLE [dbo].[PayLinks] (
    [OrderID] int  NOT NULL,
    [id] int IDENTITY(1,1) NOT NULL,
    [PayExtraction] bit  NULL,
    [PayLinkGenerated] int  NULL,
    [PayExtractionDate] datetime  NOT NULL,
    [PayLinkGeneratedDate] datetime  NULL,
    [PaymentStatus] bit  NULL,
    [SentFlag] bit  NULL
);
GO

-- Creating table 'PaymentCashFreeLogs'
CREATE TABLE [dbo].[PaymentCashFreeLogs] (
    [PaymentCashFreeLogId] int IDENTITY(1,1) NOT NULL,
    [OrderId] int  NULL,
    [InputParameters] nvarchar(max)  NULL,
    [VenderOutPut] nvarchar(max)  NULL,
    [CreatedDate] datetime  NULL,
    [OrderIdCF] nvarchar(200)  NULL,
    [OrderAmount] nvarchar(20)  NULL,
    [ReferenceId] nvarchar(255)  NULL,
    [Status] nvarchar(20)  NULL,
    [PaymentMode] nvarchar(20)  NULL,
    [Msg] nvarchar(max)  NULL,
    [TxtTime] nvarchar(30)  NULL,
    [Signature] nvarchar(max)  NULL,
    [SendStatus] nvarchar(50)  NULL,
    [MachineName] nvarchar(100)  NULL
);
GO

-- Creating table 'PaymentLinkLogs'
CREATE TABLE [dbo].[PaymentLinkLogs] (
    [PaymentLinkId] int IDENTITY(1,1) NOT NULL,
    [OrderId] int  NULL,
    [UPIString] nvarchar(max)  NULL,
    [InputParam] nvarchar(max)  NULL,
    [VendorOuput] nvarchar(max)  NULL,
    [CreatedDate] datetime  NULL,
    [CheckSum] nvarchar(max)  NULL,
    [PayUrlExpiry] nvarchar(max)  NULL,
    [PtmType] nvarchar(10)  NULL,
    [PtmLinkId] int  NULL,
    [PtmStatus] nvarchar(100)  NULL,
    [IsActive] bit  NULL
);
GO

-- Creating table 'PaymentRefunds'
CREATE TABLE [dbo].[PaymentRefunds] (
    [PaymentRefundId] int IDENTITY(1,1) NOT NULL,
    [AppLogsHookId] int  NULL,
    [OrderId] int  NULL,
    [InputParam] nvarchar(max)  NULL,
    [VendorOuput] nvarchar(max)  NULL,
    [CreatedDate] datetime  NULL,
    [TxnOrderId] nvarchar(255)  NULL,
    [TxnTimestamp] nvarchar(80)  NULL,
    [RefId] nvarchar(50)  NULL,
    [RefundId] nvarchar(200)  NULL,
    [TxnId] nvarchar(200)  NULL,
    [TxnAmount] nvarchar(10)  NULL,
    [RefundAmount] nvarchar(10)  NULL,
    [TotalRefundAmount] nvarchar(10)  NULL,
    [TxnStatus] nvarchar(50)  NULL,
    [TxnMsg] nvarchar(100)  NULL,
    [RefundStatus] nvarchar(10)  NULL,
    [IsActive] bit  NULL
);
GO

-- Creating table 'Products'
CREATE TABLE [dbo].[Products] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProductName] nvarchar(255)  NULL,
    [ProductBrandID] int  NULL,
    [ProductCategoryID] int  NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'ProductBrands'
CREATE TABLE [dbo].[ProductBrands] (
    [ProductBrandId] int IDENTITY(1,1) NOT NULL,
    [BrandName] nvarchar(50)  NULL,
    [IsDeleted] bit  NULL
);
GO

-- Creating table 'ProductCategories'
CREATE TABLE [dbo].[ProductCategories] (
    [ProductCategoryID] int IDENTITY(1,1) NOT NULL,
    [ProductCategory1] nvarchar(max)  NOT NULL,
    [ParentID] int  NOT NULL,
    [IsForFilter] bit  NOT NULL
);
GO

-- Creating table 'ProductCategoryLinks'
CREATE TABLE [dbo].[ProductCategoryLinks] (
    [ProductCategoryLinkID] int IDENTITY(1,1) NOT NULL,
    [ProductID] int  NOT NULL,
    [ProductCategoryID] int  NOT NULL
);
GO

-- Creating table 'ProductDetails'
CREATE TABLE [dbo].[ProductDetails] (
    [ProductID] int  NOT NULL,
    [ProductName] nvarchar(255)  NOT NULL,
    [Size] int  NULL,
    [Price] float  NULL,
    [CreatedDate] datetime  NULL,
    [ModifiedDate] datetime  NULL,
    [Category] nvarchar(max)  NULL,
    [ProductType] nvarchar(max)  NULL,
    [ShopItemId] nvarchar(50)  NULL,
    [ProductImage] nvarchar(255)  NULL,
    [ProductThumbImage] nvarchar(255)  NULL,
    [IsDelete] bit  NULL,
    [IsFeature] bit  NULL,
    [ProductRefID] int  NULL,
    [ProductSizeID] int  NULL
);
GO

-- Creating table 'ProductLogs'
CREATE TABLE [dbo].[ProductLogs] (
    [ProductLogId] int IDENTITY(1,1) NOT NULL,
    [ProductId] int  NULL,
    [ShopId] int  NULL,
    [OrderId] int  NULL,
    [CustomerId] int  NULL,
    [CreatedDate] datetime  NULL
);
GO

-- Creating table 'ProductNotTheres'
CREATE TABLE [dbo].[ProductNotTheres] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProductName] nvarchar(200)  NULL,
    [OrderId] int  NULL,
    [CustomerId] int  NULL,
    [CreatedDate] datetime  NULL
);
GO

-- Creating table 'ProductRecoms'
CREATE TABLE [dbo].[ProductRecoms] (
    [ProductRecomId] int IDENTITY(1,1) NOT NULL,
    [ProductId] int  NULL,
    [ShopId] int  NULL
);
GO

-- Creating table 'RoutePlans'
CREATE TABLE [dbo].[RoutePlans] (
    [id] int IDENTITY(1,1) NOT NULL,
    [OrderID] int  NOT NULL,
    [ShopID] int  NOT NULL,
    [DestPlaceID] nvarchar(128)  NULL,
    [OrigPlaceID] nvarchar(128)  NULL,
    [CustID] int  NOT NULL,
    [RoutePlanLink] nvarchar(max)  NOT NULL,
    [OrderStatusId] int  NOT NULL,
    [DeliveryStart] datetime  NOT NULL,
    [DeliveryEnd] datetime  NOT NULL,
    [DeliveryAgentId] int  NULL,
    [AssignedDate] datetime  NULL,
    [ZoneID] int  NULL,
    [SlotStart] datetime  NULL,
    [SlotEnd] datetime  NULL,
    [JobId] nvarchar(128)  NULL,
    [isOutForDelivery] bit  NULL
);
GO

-- Creating table 'RUsers'
CREATE TABLE [dbo].[RUsers] (
    [rUserId] nvarchar(128)  NOT NULL,
    [UserType] int  NULL,
    [ShopId] int  NULL,
    [DeliveryAgentId] int  NULL,
    [CreatedDate] datetime  NULL
);
GO

-- Creating table 'ShopMerchants'
CREATE TABLE [dbo].[ShopMerchants] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ShopId] int  NULL,
    [MerchantID] int  NULL
);
GO

-- Creating table 'TravelModes'
CREATE TABLE [dbo].[TravelModes] (
    [id] int  NOT NULL,
    [Mode] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'UserRefreshTokens'
CREATE TABLE [dbo].[UserRefreshTokens] (
    [UserRefreshTokenId] int IDENTITY(1,1) NOT NULL,
    [UserId] nvarchar(128)  NULL,
    [IssuedDate] datetime  NULL,
    [Token] nvarchar(128)  NULL
);
GO

-- Creating table 'UserTypes'
CREATE TABLE [dbo].[UserTypes] (
    [UserTypeId] int IDENTITY(1,1) NOT NULL,
    [UserTypeName] nvarchar(100)  NULL
);
GO

-- Creating table 'WineShops'
CREATE TABLE [dbo].[WineShops] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ShopName] nvarchar(300)  NULL,
    [Address] nvarchar(max)  NULL,
    [PhoneNo] nvarchar(max)  NULL,
    [LicNo] nvarchar(50)  NULL,
    [CLNo] nvarchar(50)  NULL,
    [CreatedDate] datetime  NULL,
    [ModifiedDate] datetime  NULL,
    [ContactPerson] nvarchar(max)  NULL,
    [GoogelCode] nvarchar(max)  NULL,
    [AvailableAgent] int  NULL,
    [Longitude] float  NULL,
    [Latitude] float  NULL,
    [PlaceId] nvarchar(128)  NULL,
    [OperationFlag] bit  NULL,
    [GST] nvarchar(50)  NULL,
    [VAT] nvarchar(50)  NULL,
    [LicPermitNo] nvarchar(100)  NULL
);
GO

-- Creating table 'AspNetUserRoles'
CREATE TABLE [dbo].[AspNetUserRoles] (
    [AspNetRoles_Id] nvarchar(128)  NOT NULL,
    [AspNetUsers_Id] nvarchar(128)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'AppLogs'
ALTER TABLE [dbo].[AppLogs]
ADD CONSTRAINT [PK_AppLogs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [AppLogsCashFreeHookId] in table 'AppLogsCashFreeHooks'
ALTER TABLE [dbo].[AppLogsCashFreeHooks]
ADD CONSTRAINT [PK_AppLogsCashFreeHooks]
    PRIMARY KEY CLUSTERED ([AppLogsCashFreeHookId] ASC);
GO

-- Creating primary key on [AppLogsPaytmHookId] in table 'AppLogsPaytmHooks'
ALTER TABLE [dbo].[AppLogsPaytmHooks]
ADD CONSTRAINT [PK_AppLogsPaytmHooks]
    PRIMARY KEY CLUSTERED ([AppLogsPaytmHookId] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetRoles'
ALTER TABLE [dbo].[AspNetRoles]
ADD CONSTRAINT [PK_AspNetRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetUserClaims'
ALTER TABLE [dbo].[AspNetUserClaims]
ADD CONSTRAINT [PK_AspNetUserClaims]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [LoginProvider], [ProviderKey], [UserId] in table 'AspNetUserLogins'
ALTER TABLE [dbo].[AspNetUserLogins]
ADD CONSTRAINT [PK_AspNetUserLogins]
    PRIMARY KEY CLUSTERED ([LoginProvider], [ProviderKey], [UserId] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetUsers'
ALTER TABLE [dbo].[AspNetUsers]
ADD CONSTRAINT [PK_AspNetUsers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [CallbackPushApiId] in table 'CallbackPushApis'
ALTER TABLE [dbo].[CallbackPushApis]
ADD CONSTRAINT [PK_CallbackPushApis]
    PRIMARY KEY CLUSTERED ([CallbackPushApiId] ASC);
GO

-- Creating primary key on [Id] in table 'Customers'
ALTER TABLE [dbo].[Customers]
ADD CONSTRAINT [PK_Customers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [CustomerAddressId] in table 'CustomerAddresses'
ALTER TABLE [dbo].[CustomerAddresses]
ADD CONSTRAINT [PK_CustomerAddresses]
    PRIMARY KEY CLUSTERED ([CustomerAddressId] ASC);
GO

-- Creating primary key on [id] in table 'CustomerContacts'
ALTER TABLE [dbo].[CustomerContacts]
ADD CONSTRAINT [PK_CustomerContacts]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [CustomerOTPVerifyId] in table 'CustomerOTPVerifies'
ALTER TABLE [dbo].[CustomerOTPVerifies]
ADD CONSTRAINT [PK_CustomerOTPVerifies]
    PRIMARY KEY CLUSTERED ([CustomerOTPVerifyId] ASC);
GO

-- Creating primary key on [Id] in table 'DeliveryAgentLogins'
ALTER TABLE [dbo].[DeliveryAgentLogins]
ADD CONSTRAINT [PK_DeliveryAgentLogins]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DeliveryAgents'
ALTER TABLE [dbo].[DeliveryAgents]
ADD CONSTRAINT [PK_DeliveryAgents]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DeliveryAgentTimes'
ALTER TABLE [dbo].[DeliveryAgentTimes]
ADD CONSTRAINT [PK_DeliveryAgentTimes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DeliveryAgentTracks'
ALTER TABLE [dbo].[DeliveryAgentTracks]
ADD CONSTRAINT [PK_DeliveryAgentTracks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DeliveryHours'
ALTER TABLE [dbo].[DeliveryHours]
ADD CONSTRAINT [PK_DeliveryHours]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DeliveryJobs'
ALTER TABLE [dbo].[DeliveryJobs]
ADD CONSTRAINT [PK_DeliveryJobs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [DeliverySlotID] in table 'DeliverySlots'
ALTER TABLE [dbo].[DeliverySlots]
ADD CONSTRAINT [PK_DeliverySlots]
    PRIMARY KEY CLUSTERED ([DeliverySlotID] ASC);
GO

-- Creating primary key on [ZoneID] in table 'DeliveryZones'
ALTER TABLE [dbo].[DeliveryZones]
ADD CONSTRAINT [PK_DeliveryZones]
    PRIMARY KEY CLUSTERED ([ZoneID] ASC);
GO

-- Creating primary key on [id] in table 'DumpRoutePlans'
ALTER TABLE [dbo].[DumpRoutePlans]
ADD CONSTRAINT [PK_DumpRoutePlans]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [InventoryId] in table 'Inventories'
ALTER TABLE [dbo].[Inventories]
ADD CONSTRAINT [PK_Inventories]
    PRIMARY KEY CLUSTERED ([InventoryId] ASC);
GO

-- Creating primary key on [Id] in table 'MerchantTrans'
ALTER TABLE [dbo].[MerchantTrans]
ADD CONSTRAINT [PK_MerchantTrans]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderDetails'
ALTER TABLE [dbo].[OrderDetails]
ADD CONSTRAINT [PK_OrderDetails]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [PK_Orders]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderStatus'
ALTER TABLE [dbo].[OrderStatus]
ADD CONSTRAINT [PK_OrderStatus]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [OrderTrackId] in table 'OrderTracks'
ALTER TABLE [dbo].[OrderTracks]
ADD CONSTRAINT [PK_OrderTracks]
    PRIMARY KEY CLUSTERED ([OrderTrackId] ASC);
GO

-- Creating primary key on [id] in table 'PayLinks'
ALTER TABLE [dbo].[PayLinks]
ADD CONSTRAINT [PK_PayLinks]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [PaymentCashFreeLogId] in table 'PaymentCashFreeLogs'
ALTER TABLE [dbo].[PaymentCashFreeLogs]
ADD CONSTRAINT [PK_PaymentCashFreeLogs]
    PRIMARY KEY CLUSTERED ([PaymentCashFreeLogId] ASC);
GO

-- Creating primary key on [PaymentLinkId] in table 'PaymentLinkLogs'
ALTER TABLE [dbo].[PaymentLinkLogs]
ADD CONSTRAINT [PK_PaymentLinkLogs]
    PRIMARY KEY CLUSTERED ([PaymentLinkId] ASC);
GO

-- Creating primary key on [PaymentRefundId] in table 'PaymentRefunds'
ALTER TABLE [dbo].[PaymentRefunds]
ADD CONSTRAINT [PK_PaymentRefunds]
    PRIMARY KEY CLUSTERED ([PaymentRefundId] ASC);
GO

-- Creating primary key on [Id] in table 'Products'
ALTER TABLE [dbo].[Products]
ADD CONSTRAINT [PK_Products]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ProductBrandId] in table 'ProductBrands'
ALTER TABLE [dbo].[ProductBrands]
ADD CONSTRAINT [PK_ProductBrands]
    PRIMARY KEY CLUSTERED ([ProductBrandId] ASC);
GO

-- Creating primary key on [ProductCategoryID] in table 'ProductCategories'
ALTER TABLE [dbo].[ProductCategories]
ADD CONSTRAINT [PK_ProductCategories]
    PRIMARY KEY CLUSTERED ([ProductCategoryID] ASC);
GO

-- Creating primary key on [ProductCategoryLinkID] in table 'ProductCategoryLinks'
ALTER TABLE [dbo].[ProductCategoryLinks]
ADD CONSTRAINT [PK_ProductCategoryLinks]
    PRIMARY KEY CLUSTERED ([ProductCategoryLinkID] ASC);
GO

-- Creating primary key on [ProductID] in table 'ProductDetails'
ALTER TABLE [dbo].[ProductDetails]
ADD CONSTRAINT [PK_ProductDetails]
    PRIMARY KEY CLUSTERED ([ProductID] ASC);
GO

-- Creating primary key on [ProductLogId] in table 'ProductLogs'
ALTER TABLE [dbo].[ProductLogs]
ADD CONSTRAINT [PK_ProductLogs]
    PRIMARY KEY CLUSTERED ([ProductLogId] ASC);
GO

-- Creating primary key on [Id] in table 'ProductNotTheres'
ALTER TABLE [dbo].[ProductNotTheres]
ADD CONSTRAINT [PK_ProductNotTheres]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ProductRecomId] in table 'ProductRecoms'
ALTER TABLE [dbo].[ProductRecoms]
ADD CONSTRAINT [PK_ProductRecoms]
    PRIMARY KEY CLUSTERED ([ProductRecomId] ASC);
GO

-- Creating primary key on [id] in table 'RoutePlans'
ALTER TABLE [dbo].[RoutePlans]
ADD CONSTRAINT [PK_RoutePlans]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [rUserId] in table 'RUsers'
ALTER TABLE [dbo].[RUsers]
ADD CONSTRAINT [PK_RUsers]
    PRIMARY KEY CLUSTERED ([rUserId] ASC);
GO

-- Creating primary key on [Id] in table 'ShopMerchants'
ALTER TABLE [dbo].[ShopMerchants]
ADD CONSTRAINT [PK_ShopMerchants]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [id] in table 'TravelModes'
ALTER TABLE [dbo].[TravelModes]
ADD CONSTRAINT [PK_TravelModes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [UserRefreshTokenId] in table 'UserRefreshTokens'
ALTER TABLE [dbo].[UserRefreshTokens]
ADD CONSTRAINT [PK_UserRefreshTokens]
    PRIMARY KEY CLUSTERED ([UserRefreshTokenId] ASC);
GO

-- Creating primary key on [UserTypeId] in table 'UserTypes'
ALTER TABLE [dbo].[UserTypes]
ADD CONSTRAINT [PK_UserTypes]
    PRIMARY KEY CLUSTERED ([UserTypeId] ASC);
GO

-- Creating primary key on [Id] in table 'WineShops'
ALTER TABLE [dbo].[WineShops]
ADD CONSTRAINT [PK_WineShops]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [AspNetRoles_Id], [AspNetUsers_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [PK_AspNetUserRoles]
    PRIMARY KEY CLUSTERED ([AspNetRoles_Id], [AspNetUsers_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [UserId] in table 'AspNetUserClaims'
ALTER TABLE [dbo].[AspNetUserClaims]
ADD CONSTRAINT [FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId'
CREATE INDEX [IX_FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]
ON [dbo].[AspNetUserClaims]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'AspNetUserLogins'
ALTER TABLE [dbo].[AspNetUserLogins]
ADD CONSTRAINT [FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId'
CREATE INDEX [IX_FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]
ON [dbo].[AspNetUserLogins]
    ([UserId]);
GO

-- Creating foreign key on [rUserId] in table 'RUsers'
ALTER TABLE [dbo].[RUsers]
ADD CONSTRAINT [FK_RUser_AspNetUsers]
    FOREIGN KEY ([rUserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [CustomerId] in table 'CustomerAddresses'
ALTER TABLE [dbo].[CustomerAddresses]
ADD CONSTRAINT [FK_CustomerAddress_Customer]
    FOREIGN KEY ([CustomerId])
    REFERENCES [dbo].[Customers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CustomerAddress_Customer'
CREATE INDEX [IX_FK_CustomerAddress_Customer]
ON [dbo].[CustomerAddresses]
    ([CustomerId]);
GO

-- Creating foreign key on [CustomerId] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [FK_CustomerOrder]
    FOREIGN KEY ([CustomerId])
    REFERENCES [dbo].[Customers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CustomerOrder'
CREATE INDEX [IX_FK_CustomerOrder]
ON [dbo].[Orders]
    ([CustomerId]);
GO

-- Creating foreign key on [CustID] in table 'DumpRoutePlans'
ALTER TABLE [dbo].[DumpRoutePlans]
ADD CONSTRAINT [FK_DumpRoutePlan_Customer]
    FOREIGN KEY ([CustID])
    REFERENCES [dbo].[Customers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DumpRoutePlan_Customer'
CREATE INDEX [IX_FK_DumpRoutePlan_Customer]
ON [dbo].[DumpRoutePlans]
    ([CustID]);
GO

-- Creating foreign key on [CustID] in table 'RoutePlans'
ALTER TABLE [dbo].[RoutePlans]
ADD CONSTRAINT [FK_RoutePlan_Customer]
    FOREIGN KEY ([CustID])
    REFERENCES [dbo].[Customers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RoutePlan_Customer'
CREATE INDEX [IX_FK_RoutePlan_Customer]
ON [dbo].[RoutePlans]
    ([CustID]);
GO

-- Creating foreign key on [ShopId] in table 'CustomerAddresses'
ALTER TABLE [dbo].[CustomerAddresses]
ADD CONSTRAINT [FK_CustomerAddress_WineShop]
    FOREIGN KEY ([ShopId])
    REFERENCES [dbo].[WineShops]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CustomerAddress_WineShop'
CREATE INDEX [IX_FK_CustomerAddress_WineShop]
ON [dbo].[CustomerAddresses]
    ([ShopId]);
GO

-- Creating foreign key on [DeliveryAgentId] in table 'DeliveryAgentLogins'
ALTER TABLE [dbo].[DeliveryAgentLogins]
ADD CONSTRAINT [FK_DeliveryAgentLogin_DeliveryAgents]
    FOREIGN KEY ([DeliveryAgentId])
    REFERENCES [dbo].[DeliveryAgents]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DeliveryAgentLogin_DeliveryAgents'
CREATE INDEX [IX_FK_DeliveryAgentLogin_DeliveryAgents]
ON [dbo].[DeliveryAgentLogins]
    ([DeliveryAgentId]);
GO

-- Creating foreign key on [DeliverySlotID] in table 'DeliveryAgents'
ALTER TABLE [dbo].[DeliveryAgents]
ADD CONSTRAINT [FK_DeliveryAgents_DeliverySlots]
    FOREIGN KEY ([DeliverySlotID])
    REFERENCES [dbo].[DeliverySlots]
        ([DeliverySlotID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DeliveryAgents_DeliverySlots'
CREATE INDEX [IX_FK_DeliveryAgents_DeliverySlots]
ON [dbo].[DeliveryAgents]
    ([DeliverySlotID]);
GO

-- Creating foreign key on [ZoneId] in table 'DeliveryAgents'
ALTER TABLE [dbo].[DeliveryAgents]
ADD CONSTRAINT [FK_DeliveryAgents_DeliveryZones]
    FOREIGN KEY ([ZoneId])
    REFERENCES [dbo].[DeliveryZones]
        ([ZoneID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DeliveryAgents_DeliveryZones'
CREATE INDEX [IX_FK_DeliveryAgents_DeliveryZones]
ON [dbo].[DeliveryAgents]
    ([ZoneId]);
GO

-- Creating foreign key on [ShopID] in table 'DeliveryAgents'
ALTER TABLE [dbo].[DeliveryAgents]
ADD CONSTRAINT [FK_DeliveryAgents_WineShop]
    FOREIGN KEY ([ShopID])
    REFERENCES [dbo].[WineShops]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DeliveryAgents_WineShop'
CREATE INDEX [IX_FK_DeliveryAgents_WineShop]
ON [dbo].[DeliveryAgents]
    ([ShopID]);
GO

-- Creating foreign key on [DeliveryAgentId] in table 'DumpRoutePlans'
ALTER TABLE [dbo].[DumpRoutePlans]
ADD CONSTRAINT [FK_DumpRoutePlan_DeliveryAgents]
    FOREIGN KEY ([DeliveryAgentId])
    REFERENCES [dbo].[DeliveryAgents]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DumpRoutePlan_DeliveryAgents'
CREATE INDEX [IX_FK_DumpRoutePlan_DeliveryAgents]
ON [dbo].[DumpRoutePlans]
    ([DeliveryAgentId]);
GO

-- Creating foreign key on [DeliveryAgentId] in table 'RoutePlans'
ALTER TABLE [dbo].[RoutePlans]
ADD CONSTRAINT [FK_RoutePlan_DeliveryAgents]
    FOREIGN KEY ([DeliveryAgentId])
    REFERENCES [dbo].[DeliveryAgents]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RoutePlan_DeliveryAgents'
CREATE INDEX [IX_FK_RoutePlan_DeliveryAgents]
ON [dbo].[RoutePlans]
    ([DeliveryAgentId]);
GO

-- Creating foreign key on [DeliveryAgentId] in table 'RUsers'
ALTER TABLE [dbo].[RUsers]
ADD CONSTRAINT [FK_RUser_DeliveryAgents]
    FOREIGN KEY ([DeliveryAgentId])
    REFERENCES [dbo].[DeliveryAgents]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RUser_DeliveryAgents'
CREATE INDEX [IX_FK_RUser_DeliveryAgents]
ON [dbo].[RUsers]
    ([DeliveryAgentId]);
GO

-- Creating foreign key on [ShopID] in table 'DeliveryZones'
ALTER TABLE [dbo].[DeliveryZones]
ADD CONSTRAINT [FK_DeliveryZones_WineShop]
    FOREIGN KEY ([ShopID])
    REFERENCES [dbo].[WineShops]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DeliveryZones_WineShop'
CREATE INDEX [IX_FK_DeliveryZones_WineShop]
ON [dbo].[DeliveryZones]
    ([ShopID]);
GO

-- Creating foreign key on [OrderID] in table 'DumpRoutePlans'
ALTER TABLE [dbo].[DumpRoutePlans]
ADD CONSTRAINT [FK_DumpRoutePlan_Orders]
    FOREIGN KEY ([OrderID])
    REFERENCES [dbo].[Orders]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DumpRoutePlan_Orders'
CREATE INDEX [IX_FK_DumpRoutePlan_Orders]
ON [dbo].[DumpRoutePlans]
    ([OrderID]);
GO

-- Creating foreign key on [OrderStatusId] in table 'DumpRoutePlans'
ALTER TABLE [dbo].[DumpRoutePlans]
ADD CONSTRAINT [FK_DumpRoutePlan_OrderStatus]
    FOREIGN KEY ([OrderStatusId])
    REFERENCES [dbo].[OrderStatus]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DumpRoutePlan_OrderStatus'
CREATE INDEX [IX_FK_DumpRoutePlan_OrderStatus]
ON [dbo].[DumpRoutePlans]
    ([OrderStatusId]);
GO

-- Creating foreign key on [ShopID] in table 'DumpRoutePlans'
ALTER TABLE [dbo].[DumpRoutePlans]
ADD CONSTRAINT [FK_DumpRoutePlan_WineShop]
    FOREIGN KEY ([ShopID])
    REFERENCES [dbo].[WineShops]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DumpRoutePlan_WineShop'
CREATE INDEX [IX_FK_DumpRoutePlan_WineShop]
ON [dbo].[DumpRoutePlans]
    ([ShopID]);
GO

-- Creating foreign key on [ProductID] in table 'Inventories'
ALTER TABLE [dbo].[Inventories]
ADD CONSTRAINT [FK_Inventory_ProductDetails]
    FOREIGN KEY ([ProductID])
    REFERENCES [dbo].[ProductDetails]
        ([ProductID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Inventory_ProductDetails'
CREATE INDEX [IX_FK_Inventory_ProductDetails]
ON [dbo].[Inventories]
    ([ProductID]);
GO

-- Creating foreign key on [ShopID] in table 'Inventories'
ALTER TABLE [dbo].[Inventories]
ADD CONSTRAINT [FK_Inventory_WineShop]
    FOREIGN KEY ([ShopID])
    REFERENCES [dbo].[WineShops]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Inventory_WineShop'
CREATE INDEX [IX_FK_Inventory_WineShop]
ON [dbo].[Inventories]
    ([ShopID]);
GO

-- Creating foreign key on [MerchantID] in table 'ShopMerchants'
ALTER TABLE [dbo].[ShopMerchants]
ADD CONSTRAINT [FK_ShopMerchant_MerchantTrans]
    FOREIGN KEY ([MerchantID])
    REFERENCES [dbo].[MerchantTrans]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ShopMerchant_MerchantTrans'
CREATE INDEX [IX_FK_ShopMerchant_MerchantTrans]
ON [dbo].[ShopMerchants]
    ([MerchantID]);
GO

-- Creating foreign key on [OrderId] in table 'OrderDetails'
ALTER TABLE [dbo].[OrderDetails]
ADD CONSTRAINT [FK_OrderDetail_Orders]
    FOREIGN KEY ([OrderId])
    REFERENCES [dbo].[Orders]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderDetail_Orders'
CREATE INDEX [IX_FK_OrderDetail_Orders]
ON [dbo].[OrderDetails]
    ([OrderId]);
GO

-- Creating foreign key on [ProductID] in table 'OrderDetails'
ALTER TABLE [dbo].[OrderDetails]
ADD CONSTRAINT [FK_OrderDetail_ProductDetails1]
    FOREIGN KEY ([ProductID])
    REFERENCES [dbo].[ProductDetails]
        ([ProductID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderDetail_ProductDetails1'
CREATE INDEX [IX_FK_OrderDetail_ProductDetails1]
ON [dbo].[OrderDetails]
    ([ProductID]);
GO

-- Creating foreign key on [ShopID] in table 'OrderDetails'
ALTER TABLE [dbo].[OrderDetails]
ADD CONSTRAINT [FK_OrderDetail_WineShop]
    FOREIGN KEY ([ShopID])
    REFERENCES [dbo].[WineShops]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderDetail_WineShop'
CREATE INDEX [IX_FK_OrderDetail_WineShop]
ON [dbo].[OrderDetails]
    ([ShopID]);
GO

-- Creating foreign key on [OrderStatusId] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [FK_Orders_OrderStatus]
    FOREIGN KEY ([OrderStatusId])
    REFERENCES [dbo].[OrderStatus]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Orders_OrderStatus'
CREATE INDEX [IX_FK_Orders_OrderStatus]
ON [dbo].[Orders]
    ([OrderStatusId]);
GO

-- Creating foreign key on [ShopID] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [FK_Orders_WineShop]
    FOREIGN KEY ([ShopID])
    REFERENCES [dbo].[WineShops]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Orders_WineShop'
CREATE INDEX [IX_FK_Orders_WineShop]
ON [dbo].[Orders]
    ([ShopID]);
GO

-- Creating foreign key on [OrderID] in table 'PayLinks'
ALTER TABLE [dbo].[PayLinks]
ADD CONSTRAINT [FK_PayLink_Orders]
    FOREIGN KEY ([OrderID])
    REFERENCES [dbo].[Orders]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PayLink_Orders'
CREATE INDEX [IX_FK_PayLink_Orders]
ON [dbo].[PayLinks]
    ([OrderID]);
GO

-- Creating foreign key on [OrderID] in table 'RoutePlans'
ALTER TABLE [dbo].[RoutePlans]
ADD CONSTRAINT [FK_RoutePlan_Orders]
    FOREIGN KEY ([OrderID])
    REFERENCES [dbo].[Orders]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RoutePlan_Orders'
CREATE INDEX [IX_FK_RoutePlan_Orders]
ON [dbo].[RoutePlans]
    ([OrderID]);
GO

-- Creating foreign key on [StatusId] in table 'OrderTracks'
ALTER TABLE [dbo].[OrderTracks]
ADD CONSTRAINT [FK_OrderTrack_OrderStatus]
    FOREIGN KEY ([StatusId])
    REFERENCES [dbo].[OrderStatus]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderTrack_OrderStatus'
CREATE INDEX [IX_FK_OrderTrack_OrderStatus]
ON [dbo].[OrderTracks]
    ([StatusId]);
GO

-- Creating foreign key on [OrderStatusId] in table 'RoutePlans'
ALTER TABLE [dbo].[RoutePlans]
ADD CONSTRAINT [FK_RoutePlan_OrderStatus]
    FOREIGN KEY ([OrderStatusId])
    REFERENCES [dbo].[OrderStatus]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RoutePlan_OrderStatus'
CREATE INDEX [IX_FK_RoutePlan_OrderStatus]
ON [dbo].[RoutePlans]
    ([OrderStatusId]);
GO

-- Creating foreign key on [ProductBrandID] in table 'Products'
ALTER TABLE [dbo].[Products]
ADD CONSTRAINT [FK_Product_ProductBrand1]
    FOREIGN KEY ([ProductBrandID])
    REFERENCES [dbo].[ProductBrands]
        ([ProductBrandId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Product_ProductBrand1'
CREATE INDEX [IX_FK_Product_ProductBrand1]
ON [dbo].[Products]
    ([ProductBrandID]);
GO

-- Creating foreign key on [ProductCategoryID] in table 'Products'
ALTER TABLE [dbo].[Products]
ADD CONSTRAINT [FK_Product_ProductCategory1]
    FOREIGN KEY ([ProductCategoryID])
    REFERENCES [dbo].[ProductCategories]
        ([ProductCategoryID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Product_ProductCategory1'
CREATE INDEX [IX_FK_Product_ProductCategory1]
ON [dbo].[Products]
    ([ProductCategoryID]);
GO

-- Creating foreign key on [ProductRefID] in table 'ProductDetails'
ALTER TABLE [dbo].[ProductDetails]
ADD CONSTRAINT [FK_ProductDetails_Product]
    FOREIGN KEY ([ProductRefID])
    REFERENCES [dbo].[Products]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductDetails_Product'
CREATE INDEX [IX_FK_ProductDetails_Product]
ON [dbo].[ProductDetails]
    ([ProductRefID]);
GO

-- Creating foreign key on [ShopID] in table 'RoutePlans'
ALTER TABLE [dbo].[RoutePlans]
ADD CONSTRAINT [FK_RoutePlan_WineShop]
    FOREIGN KEY ([ShopID])
    REFERENCES [dbo].[WineShops]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RoutePlan_WineShop'
CREATE INDEX [IX_FK_RoutePlan_WineShop]
ON [dbo].[RoutePlans]
    ([ShopID]);
GO

-- Creating foreign key on [UserType] in table 'RUsers'
ALTER TABLE [dbo].[RUsers]
ADD CONSTRAINT [FK_RUser_UserType]
    FOREIGN KEY ([UserType])
    REFERENCES [dbo].[UserTypes]
        ([UserTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RUser_UserType'
CREATE INDEX [IX_FK_RUser_UserType]
ON [dbo].[RUsers]
    ([UserType]);
GO

-- Creating foreign key on [ShopId] in table 'RUsers'
ALTER TABLE [dbo].[RUsers]
ADD CONSTRAINT [FK_RUser_WineShop]
    FOREIGN KEY ([ShopId])
    REFERENCES [dbo].[WineShops]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RUser_WineShop'
CREATE INDEX [IX_FK_RUser_WineShop]
ON [dbo].[RUsers]
    ([ShopId]);
GO

-- Creating foreign key on [AspNetRoles_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [FK_AspNetUserRoles_AspNetRole]
    FOREIGN KEY ([AspNetRoles_Id])
    REFERENCES [dbo].[AspNetRoles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [AspNetUsers_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [FK_AspNetUserRoles_AspNetUser]
    FOREIGN KEY ([AspNetUsers_Id])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AspNetUserRoles_AspNetUser'
CREATE INDEX [IX_FK_AspNetUserRoles_AspNetUser]
ON [dbo].[AspNetUserRoles]
    ([AspNetUsers_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------