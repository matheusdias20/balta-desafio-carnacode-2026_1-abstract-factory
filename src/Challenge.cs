using System;

namespace DesignPatternChallenge
{
    // ============================
    // 1. Interfaces for Products
    // ============================
    public interface IPaymentValidator
    {
        bool ValidateCard(string cardNumber);
    }

    public interface IPaymentProcessor
    {
        string ProcessTransaction(decimal amount, string cardNumber);
    }

    public interface IPaymentLogger
    {
        void Log(string message);
    }

    // ============================
    // 2. Abstract Factory
    // ============================
    public interface IPaymentFactory
    {
        IPaymentValidator CreateValidator();
        IPaymentProcessor CreateProcessor();
        IPaymentLogger CreateLogger();
    }

    // ============================
    // 3. Concrete Products
    // ============================

    // --- PagSeguro ---
    public class PagSeguroValidator : IPaymentValidator
    {
        public bool ValidateCard(string cardNumber) 
        {
            Console.WriteLine("PagSeguro: Validando cartão...");
            return cardNumber.Length == 16;
        }
    }

    public class PagSeguroProcessor : IPaymentProcessor
    {
        public string ProcessTransaction(decimal amount, string cardNumber)
        {
            Console.WriteLine($"PagSeguro: Processando R$ {amount}...");
            return $"PAGSEG-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }

    public class PagSeguroLogger : IPaymentLogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[PagSeguro Log] {DateTime.Now}: {message}");
        }
    }

    // --- MercadoPago ---
    public class MercadoPagoValidator : IPaymentValidator
    {
        public bool ValidateCard(string cardNumber)
        {
            Console.WriteLine("MercadoPago: Validando cartão...");
            return cardNumber.Length == 16 && cardNumber.StartsWith("5");
        }
    }

    public class MercadoPagoProcessor : IPaymentProcessor
    {
        public string ProcessTransaction(decimal amount, string cardNumber)
        {
            Console.WriteLine($"MercadoPago: Processando R$ {amount}...");
            return $"MP-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }

    public class MercadoPagoLogger : IPaymentLogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[MercadoPago Log] {DateTime.Now}: {message}");
        }
    }

    // --- Stripe ---
    public class StripeValidator : IPaymentValidator
    {
        public bool ValidateCard(string cardNumber)
        {
            Console.WriteLine("Stripe: Validando cartão...");
            return cardNumber.Length == 16 && cardNumber.StartsWith("4");
        }
    }

    public class StripeProcessor : IPaymentProcessor
    {
        public string ProcessTransaction(decimal amount, string cardNumber)
        {
            Console.WriteLine($"Stripe: Processando ${amount}...");
            return $"STRIPE-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }

    public class StripeLogger : IPaymentLogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[Stripe Log] {DateTime.Now}: {message}");
        }
    }

    // ============================
    // 4. Concrete Factories
    // ============================
    public class PagSeguroFactory : IPaymentFactory
    {
        public IPaymentValidator CreateValidator() => new PagSeguroValidator();
        public IPaymentProcessor CreateProcessor() => new PagSeguroProcessor();
        public IPaymentLogger CreateLogger() => new PagSeguroLogger();
    }

    public class MercadoPagoFactory : IPaymentFactory
    {
        public IPaymentValidator CreateValidator() => new MercadoPagoValidator();
        public IPaymentProcessor CreateProcessor() => new MercadoPagoProcessor();
        public IPaymentLogger CreateLogger() => new MercadoPagoLogger();
    }

    public class StripeFactory : IPaymentFactory
    {
        public IPaymentValidator CreateValidator() => new StripeValidator();
        public IPaymentProcessor CreateProcessor() => new StripeProcessor();
        public IPaymentLogger CreateLogger() => new StripeLogger();
    }

    // ============================
    // 5. Client (PaymentService)
    // ============================
    public class PaymentService
    {
        private readonly IPaymentValidator _validator;
        private readonly IPaymentProcessor _processor;
        private readonly IPaymentLogger _logger;
        private readonly string _gatewayName;

        public PaymentService(IPaymentFactory factory)
        {
            _validator = factory.CreateValidator();
            _processor = factory.CreateProcessor();
            _logger = factory.CreateLogger();
            
            // Just for output matching the original code style
            if (factory is PagSeguroFactory) _gatewayName = "PagSeguro";
            else if (factory is MercadoPagoFactory) _gatewayName = "MercadoPago";
            else if (factory is StripeFactory) _gatewayName = "Stripe";
            else _gatewayName = "Gateway";
        }

        public void ProcessPayment(decimal amount, string cardNumber)
        {
            if (!_validator.ValidateCard(cardNumber))
            {
                Console.WriteLine($"{_gatewayName}: Cartão inválido");
                return;
            }

            var result = _processor.ProcessTransaction(amount, cardNumber);
            _logger.Log($"Transação processada: {result}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Sistema de Pagamentos ===\n");

            // PagSeguro
            var pagSeguroFactory = new PagSeguroFactory();
            var pagSeguroService = new PaymentService(pagSeguroFactory);
            pagSeguroService.ProcessPayment(150.00m, "1234567890123456");

            Console.WriteLine();

            // MercadoPago
            var mercadoPagoFactory = new MercadoPagoFactory();
            var mercadoPagoService = new PaymentService(mercadoPagoFactory);
            mercadoPagoService.ProcessPayment(200.00m, "5234567890123456");

            Console.WriteLine();
        }
    }
}
