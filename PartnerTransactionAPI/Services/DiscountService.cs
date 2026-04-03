namespace PartnerTransactionAPI.Services
{
    public class DiscountService
    {
        public (long discount, long finalAmount) Calculate(long totalAmountCents)
        {
            // Convert cents to MYR for rule comparison
            decimal amountMyr = totalAmountCents / 100m;

            decimal baseRate = 0m;

            if (amountMyr < 200)
                baseRate = 0m;
            else if (amountMyr <= 500)
                baseRate = 0.05m;   // 5%
            else if (amountMyr <= 800)
                baseRate = 0.07m;   // 7%
            else if (amountMyr <= 1200)
                baseRate = 0.10m;   // 10%
            else
                baseRate = 0.15m;   // 15%

            decimal conditionalRate = 0m;

            // Condition A: totalAmount (MYR) is prime AND above MYR 500
            if (amountMyr > 500 && IsPrime((long)amountMyr))
                conditionalRate += 0.08m;  // +8%

            // Condition B: ends in digit 5 AND above MYR 900
            if (amountMyr > 900 && (long)amountMyr % 10 == 5)
                conditionalRate += 0.10m;  // +10%

            // Step 3: Apply 20% cap 
            decimal totalRate = baseRate + conditionalRate;
            if (totalRate > 0.20m)
                totalRate = 0.20m;

            // Step 4: Calculate amounts (stay in cents) 
            long discountCents = (long)Math.Floor(totalAmountCents * totalRate);
            long finalCents    = totalAmountCents - discountCents;

            return (discountCents, finalCents);
            
        }

        // Check if a number is prime
        // Prime = only divisible by 1 and itself (e.g. 2, 3, 5, 7, 11...)
        private bool IsPrime(long n)
        {
            if (n < 2) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false;  // even numbers can't be prime

            // Only need to check up to square root of n
            for (long i = 3; i * i <= n; i += 2)
            {
                if (n % i == 0) return false;
            }
            return true;
        }
    }
}

