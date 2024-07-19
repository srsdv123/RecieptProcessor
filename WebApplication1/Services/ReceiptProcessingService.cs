namespace RecieptProcessor.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using RecieptProcessor.Models;
        public class ReceiptProcessingService
        {
            private readonly ConcurrentDictionary<string, ProcessedReceipt> _receipts = new ConcurrentDictionary<string, ProcessedReceipt>();

            public string ProcessReceipt(Receipt receipt)
            {
                var points = CalculatePoints(receipt);
                var id = Guid.NewGuid().ToString();

                var processedReceipt = new ProcessedReceipt { Id = id, Points = points };
                _receipts.TryAdd(id, processedReceipt);

                return id;
            }

            public ProcessedReceipt GetReceiptPoints(string id)
            {
                _receipts.TryGetValue(id, out var processedReceipt);
                return processedReceipt;
            }

            private int CalculatePoints(Receipt receipt)
            {
                int points = 0;

                // Rule 1: One point for every alphanumeric character in the retailer name
                var alphanumericCount = Regex.Matches(receipt.Retailer, "[a-zA-Z0-9]").Count;
                points += alphanumericCount;

                // Rule 2: 50 points if the total is a round dollar amount with no cents
                if (decimal.TryParse(receipt.Total, out var total) && total == Math.Floor(total))
                {
                    points += 50;
                }

                // Rule 3: 25 points if the total is a multiple of 0.25
                if (total % 0.25m == 0)
                {
                    points += 25;
                }

                // Rule 4: 5 points for every two items on the receipt
                points += (receipt.Items.Count / 2) * 5;

                // Rule 5: Points based on item description length
                foreach (var item in receipt.Items)
                {
                    var trimmedLength = item.ShortDescription.Trim().Length;
                    if (trimmedLength % 3 == 0)
                    {
                        if (decimal.TryParse(item.Price, out var itemPrice))
                        {
                            points += (int)Math.Ceiling(itemPrice * 0.2m);
                        }
                    }
                }

                // Rule 6: 6 points if the day in the purchase date is odd
                if (DateTime.TryParse(receipt.PurchaseDate, out var purchaseDate) && purchaseDate.Day % 2 != 0)
                {
                    points += 6;
                }

                // Rule 7: 10 points if the time of purchase is after 2:00pm and before 4:00pm
                if (TimeSpan.TryParse(receipt.PurchaseTime, out var purchaseTime) &&
                    purchaseTime >= new TimeSpan(14, 0, 0) && purchaseTime < new TimeSpan(16, 0, 0))
                {
                    points += 10;
                }

                return points;
            }
        }
    }

