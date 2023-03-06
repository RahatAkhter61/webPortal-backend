using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.General
{
    public class Enums
    {

        public struct CifStatuses
        {
            public const string INACTIVE = "IA";
            public const string ACTIVE = "AC";
            public const string CANCEL = "CL";
            public const string NEW = "NEW";
        }
        public struct UserCategoryCode
        {
            public const string Employee = "01";
            public const string Company = "00";
            public const string Other = "02";

        }

        public struct CompanyProductsCode
        {
            public const string Wps = "3500";
            public const string nonWps = "3600";
        }

        public struct TransactionTypes
        {
            public const string UploadDepositSlip = "Upload Deposit Slip";
            public const string SIFGeneration = "SIF Generation";
            public const string CardLoad = "Card Load";
            public const string CashReload = "Cash Reload";
            public const string ATMWithdrawl = "ATM Withdrawl";
            public const string Reversal = "Reversal";
            public const string ManualDebit = "Manual Debit";
            public const string Withdrawl = "Withdrawl";
            public const string KPAPIDebit = "KP API Debit";
            public const string KPAPIDebitFee = "KP API Debit Fee";
            public const string OtherBankSIF = "Other Bank SIF";
            public const string CashLoadCharges = "Cash Load Charges";
            public const string MasterCardWITHDRAWALFEE = "MasterCard WITHDRAWAL FEE";
            public const string MasterCardBALANCEINQUIRYFEE = "MasterCard BALANCE INQUIRY FEE";
            public const string MasterCardWITHDRAWALDECLINEFEE = "MasterCard-WITHDRAWAL DECLINE FEE";
            public const string CardReplacementCharge = "Card Replacement Charge";
            public const string UAESWITCHWITHDRAWALFEE = "UAESWITCH WITHDRAWAL FEE";
            public const string UAESWITCHWITHDRAWALDECLINEFEE = "UAESWITCH WITHDRAWAL DECLINE FEE";
            public const string UAESWITCHBALANCEINQUIRYFEE = "UAESWITCH BALANCE INQUIRY FEE";
            public const string MCLatePresentmentReversal = "MC Late Presentment - Reversal";
            public const string VASRefunds = "VAS Refunds";
            public const string CompanyFundtransfer = "Company Fund transfer";
            public const string AddingFunds = "Adding Funds";
            public const string CompanyCharge = "Company Charge";
            public const string RTCClaimed = "RTC Claimed";


        }

        public struct Chargetypes
        {
            public const string CardIssuance = "Card Issuance";
            public const string CardReplacement = "Card Replacement";
            public const string SMSSubscription = "SMS Subscription";
            public const string FileOpenRate = "File Open Rate";
            public const string PERSIFPerMonthCharges = "PER SIF Per Month Charges";
            public const string CardMonthlyFee = "Card Monthly Fee";
            public const string CardDormancyFeeperMonth = "Card Dormancy Fee (per month)";
            public const string CardCancellation = "Card Cancellation";
            public const string CardCancellationWithRefund = "Card Cancellation with Refund";
            public const string CardLoadNonWPS = "Card Load (Non WPS)";
            public const string PerFile = "Per File";
            public const string PerRecord = "Per Record";
        }

        public struct SystemSettings
        {
            public const string VAT = "Vat_Val";
            public const string AdvanceMaxSalary = "AdvanceMaxSalary";
            public const string AdvanceMinSalary = "AdvanceMinSalary";
            public const string MaxAmountOfEligibility = "MaxAmountOfEligibility";

        }
        public enum SystemSetting
        {
            Vat_Val, TransactionHistoryTimeOut, CustomerEnquiryTimeOut,
            AdvanceAverageSalary
        }

        public struct StatusTypes
        {
            public const string Ecrow = "Escrow Statuses";
            //public const string Pending = "PENDING";
            public const string AdvanceSalary = "AdvanceSalary";
            public const string DebitRefund = "Debit Refund";
            public const string RefundApproval = "Refund Approval";
            public const string WithFee = "With Fee";
            public const string WithoutFee = "Without Fee";
            public const string Partial = "Partial";
            public const string Pending = "Pending";
            public const string Accepted = "Accepted";
            public const string Rejected = "Rejected";
            public const string Hold = "Hold";
            public const string CommonStatus = "Common Status";
            public const string Approved = "Approved";
            public const string Disbursed = "Disbursed";
            public const string Error = "Error";

            public const string HomeVisitPlan = "HomeVisitPlan";
            public const string PurposeofLoan = "PurposeofLoan";
            public const string JobLeavingPlan = "JobLeavingPlan";
        }

        public struct EscrowStatus
        {
            public const string Freez = "Freez";
            public const string Released = "Released";
        }

        public struct CompanyStatus
        {
            public const int Freez = 1;
            public const int Released = 2;
            public const int success = 3;
            public const int retry = 4;
            public const int cancelled = 5;
            public const int failure = 6;
            public const int pending = 7;
            public const int WhiteList = 8;
            public const int BlackList = 9;

            public const string New = "New";
            public const string CKP = "CKP";
            public const string CKF = "CKF";

        }
        public struct ChargesTypeCode
        {
            public const string CardIssuance = "C001";
            public const string CardReplacement = "C002";
            public const string SMSSubscription = "C003";
            public const string FileOpenRate = "C004";
            public const string PERSIFPerMonthCharges = "C005";
            public const string CardMonthlyFee = "C006";
            public const string CardDormancyFeepermonth = "C007";
            public const string CardCancellation = "C008";
            public const string CardCancellationwithRefund = "C010";
            public const string CardLoadNonWPS = "C011";
            public const string PerRecord = "C012";
            public const string PerFile = "C013";
        }

        public struct AdvanceSalaryStatus
        {
            public const string AdvanceSalary = "Advance Salary";
            public const string PaymentType = "Payment Type";
            public const string Approve = "Approve";
            public const string Reject = "Reject";
            public const string Pending = "Pending";
            public const string SendtoBank = "Send to Bank";
            public const string ComplianceView = "ComplianceView";
            public const string AdvanceCharges = "AdvanceCharges";
            public const string AdvanceFee = "AdvanceFee";
            public const string FullyRecieve = "Fully Recieve";

        }

        public struct CompanyWhiteListing
        {
            public const string CompanyWhitelisting = "CompanyWhitelisting";
            public const string CompanyLiers = "CompanyLiers";

        }


        public struct ThirdPartyStatus
        {
            public const string CentivLoadFunds = "CENTIV_LOAD_FUNDS";
            public const string DepositFileUpload = "DEPOSIT_FILE_UPLOAD";
            public const string ApprovedByKamelPay = "APPROVED_BY_KAMELPAY";
            public const string RejectByKamelPay = "REJECTED_BY_KAMELPAY";
        }

        public struct TransactionTypeCode
        {
            public const string sifNakClaimed = "SNC";
        }

        public struct ComapnyWhiteStatus
        {
            public const string WhiteList = "WhiteList";
            public const string BlackList = "BlackList";
        }

        public struct EmployeeWhitelistingStatus
        {
            public const string WhiteList = "WhiteList";
            public const string BlackList = "BlackList";
        }

        public struct EmployeeWhitelistingStatusType
        {
            public const string EmployeeWhitelisting = "EmployeeWhitelisting";
        }

        public struct AdvanceEligibiltyStatus
        {
            public const string No = "No";
            public const string Yes = "Yes";
            public const string Eligible = "Eligible";

        }
        public struct AlertSmsType
        {
            public const string ADSAL = "ADSAL";
        }

        public struct PCMSStatusType
        {
            public const string Statustype = "Status_type";
            public const string Cardload = "Cardload";
        }

        public struct PCMSStatus
        {
            public const string InProcess = "In-Process";
            public const string Pending = "Pending";
            public const string Error = "Error";
            public const string Disbursed = "Disbursed";
        }


    }
}