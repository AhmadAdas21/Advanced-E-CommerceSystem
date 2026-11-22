
using System.Diagnostics;
using System.Net.Http.Json;

// ===========================
// TEST CONFIGURATION
// ===========================

const int totalRequests = 500;
const int customerId = 1; // Change to an existing CustomerId
const string sku = "CONC-TEST-001";

const string apiUrl = "http://localhost:8085/api/orders";

// ===========================
// HTTP CLIENT
// ===========================

using var handler = new SocketsHttpHandler
{
    MaxConnectionsPerServer = 500
};

using var client = new HttpClient(handler)
{
    Timeout = TimeSpan.FromSeconds(120)
};

// All requests wait for the same start signal
var startSignal = new TaskCompletionSource<bool>(
    TaskCreationOptions.RunContinuationsAsynchronously);

// ===========================
// PREPARE 500 REQUESTS
// ===========================

var tasks = Enumerable.Range(1, totalRequests)
    .Select(async requestNumber =>
    {
        await startSignal.Task;

        try
        {
            var requestBody = new
            {
                CustomerId = customerId,

                // Unique ID for every purchase
                RequestId = Guid.NewGuid(),

                Items = new[]
                {
                    new
                    {
                        SKU = sku,
                        Quantity = 1
                    }
                }
            };

            using var response = await client.PostAsJsonAsync(
                apiUrl,
                requestBody);

            int statusCode = (int)response.StatusCode;

            string details = statusCode is 201 or 409
                ? ""
                : await response.Content.ReadAsStringAsync();

            return new TestResult(
                requestNumber,
                statusCode,
                details);
        }
        catch (Exception ex)
        {
            return new TestResult(
                requestNumber,
                0,
                ex.Message);
        }
    })
    .ToArray();

// ===========================
// START CONCURRENCY TEST
// ===========================

Console.WriteLine("==================================");
Console.WriteLine("    CONCURRENCY CHALLENGE");
Console.WriteLine("==================================");

Console.WriteLine($"Total Requests: {totalRequests}");
Console.WriteLine("Initial Stock Expected: 25");
Console.WriteLine($"Product SKU: {sku}");

Console.WriteLine("\nStarting concurrent purchases...");

var stopwatch = Stopwatch.StartNew();

// Release all waiting tasks
startSignal.SetResult(true);

var results = await Task.WhenAll(tasks);

stopwatch.Stop();

// ===========================
// ANALYZE RESULTS
// ===========================

int successful = results.Count(r => r.StatusCode == 201);
int rejected = results.Count(r => r.StatusCode == 409);
int errors = results.Length - successful - rejected;

Console.WriteLine("\n==================================");
Console.WriteLine("          TEST RESULTS");
Console.WriteLine("==================================");

Console.WriteLine($"Total Requests:    {results.Length}");
Console.WriteLine($"Successful (201):  {successful}");
Console.WriteLine($"Rejected (409):    {rejected}");
Console.WriteLine($"Other Errors:      {errors}");
Console.WriteLine($"Duration:          {stopwatch.Elapsed.TotalSeconds:F2} seconds");

// ===========================
// HTTP STATUS DISTRIBUTION
// ===========================

Console.WriteLine("\nHTTP STATUS DISTRIBUTION:");

foreach (var group in results
    .GroupBy(r => r.StatusCode)
    .OrderBy(g => g.Key))
{
    Console.WriteLine(
        $"HTTP {group.Key}: {group.Count()} requests");
}

// ===========================
// PRINT UNEXPECTED ERRORS
// ===========================

if (errors > 0)
{
    Console.WriteLine("\nUNEXPECTED ERRORS:");

    foreach (var error in results
        .Where(r => r.StatusCode != 201 &&
                    r.StatusCode != 409)
        .Take(10))
    {
        Console.WriteLine(
            $"Request #{error.RequestNumber}: " +
            $"HTTP {error.StatusCode} - {error.Details}");
    }
}

// ===========================
// FINAL HTTP VERDICT
// ===========================

Console.WriteLine("\n==================================");

if (successful == 25 && rejected == 475 && errors == 0)
{
    Console.WriteLine("HTTP TEST PASSED!");
    Console.WriteLine("Exactly 25 purchases succeeded.");
    Console.WriteLine("475 requests were rejected.");
}
else
{
    Console.WriteLine("TEST NEEDS INVESTIGATION.");
    Console.WriteLine("Review API logs and database.");
}

Console.WriteLine("==================================");

record TestResult(
    int RequestNumber,
    int StatusCode,
    string Details);
