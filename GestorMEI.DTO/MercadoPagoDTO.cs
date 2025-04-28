using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.DTO
{
    public class MercadoPagoDTO
    {
        public string? PaymentType { get; set; }
        public string? SelectedPaymentMethod { get; set; }
        public CardFormData? FormData { get; set; }
    }
    public class CardFormData
    {
        public decimal Transaction_Amount { get; set; }
        public string? Token { get; set; }
        public string? Description { get; set; }
        public int Installments { get; set; }
        public string? Payment_Method_Id { get; set; }
        public CardFormPayer? Payer { get; set; }
    }

    public class CardFormPayer
    {
        public string? Email { get; set; }
        public string? CardHolderName { get; set; }
        public Identification? Identification { get; set; }

    }

    public class Identification
    {
        public string? Type { get; set; }
        public string? Number { get; set; }
    }
}
