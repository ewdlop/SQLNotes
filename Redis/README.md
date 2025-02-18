# Redis

## Notes

<https://redis.io/learn>

### .Net

<https://redis.io/docs/latest/develop/clients/dotnet/>

## data-types

<https://redis.io/docs/latest/develop/data-types/>

#### hashes 

<https://redis.io/docs/latest/develop/data-types/hashes/>

```csharp

using NRedisStack.Tests;
using StackExchange.Redis;

public class HashExample
{
    public void run()
    {
        var muxer = ConnectionMultiplexer.Connect("localhost:6379");
        var db = muxer.GetDatabase();
        db.KeyDelete("bike:1");
        db.HashSet("bike:1", new HashEntry[]
        {
            new HashEntry("model", "Deimos"),
            new HashEntry("brand", "Ergonom"),
            new HashEntry("type", "Enduro bikes"),
            new HashEntry("price", 4972)
        });

        Console.WriteLine("Hash Created");
        // Hash Created

        var model = db.HashGet("bike:1", "model");
        Console.WriteLine($"Model: {model}");
        // Model: Deimos

        var price = db.HashGet("bike:1", "price");
        Console.WriteLine($"Price: {price}");
        // Price: 4972

        var bike = db.HashGetAll("bike:1");
        Console.WriteLine("bike:1");
        Console.WriteLine(string.Join("\n", bike.Select(b => $"{b.Name}: {b.Value}")));
        // Bike:1:
        // model: Deimos
        // brand: Ergonom
        // type: Enduro bikes
        // price: 4972


        var values = db.HashGet("bike:1", new RedisValue[] { "model", "price" });
        Console.WriteLine(string.Join(" ", values));
        // Deimos 4972

        var newPrice = db.HashIncrement("bike:1", "price", 100);
        Console.WriteLine($"New price: {newPrice}");
        // New price: 5072

        newPrice = db.HashIncrement("bike:1", "price", -100);
        Console.WriteLine($"New price: {newPrice}");
        // New price: 4972

        var rides = db.HashIncrement("bike:1", "rides");
        Console.WriteLine($"Rides: {rides}");
        // Rides: 1

        rides = db.HashIncrement("bike:1", "rides");
        Console.WriteLine($"Rides: {rides}");
        // Rides: 2

        rides = db.HashIncrement("bike:1", "rides");
        Console.WriteLine($"Rides: {rides}");
        // Rides: 3

        var crashes = db.HashIncrement("bike:1", "crashes");
        Console.WriteLine($"Crashes: {crashes}");
        // Crashes: 1

        var owners = db.HashIncrement("bike:1", "owners");
        Console.WriteLine($"Owners: {owners}");
        // Owners: 1

        var stats = db.HashGet("bike:1", new RedisValue[] { "crashes", "owners" });
        Console.WriteLine($"Bike stats: crashes={stats[0]}, owners={stats[1]}");
        // Bike stats: crashes=1, owners=1
    }
}
```

### probabilistic

<https://redis.io/docs/latest/develop/data-types/probabilistic/>

##### hyperloglogs

<https://redis.io/docs/latest/develop/data-types/probabilistic/hyperloglogs/>

```cshar

using NRedisStack.Tests;
using StackExchange.Redis;



public class Hll_tutorial
{
    public void run()
    {
        var muxer = ConnectionMultiplexer.Connect("localhost:6379");
        var db = muxer.GetDatabase();


        bool res1 = db.HyperLogLogAdd("{bikes}", new RedisValue[] { "Hyperion", "Deimos", "Phoebe", "Quaoar" });
        Console.WriteLine(res1);    // >>> True

        long res2 = db.HyperLogLogLength("{bikes}");
        Console.WriteLine(res2);    // >>> 4

        bool res3 = db.HyperLogLogAdd("commuter_{bikes}", new RedisValue[] { "Salacia", "Mimas", "Quaoar" });
        Console.WriteLine(res3);    // >>> True

        db.HyperLogLogMerge("all_{bikes}", "{bikes}", "commuter_{bikes}");
        long res4 = db.HyperLogLogLength("all_{bikes}");
        Console.WriteLine(res4);    // >>> 6

        // Tests for 'pfadd' step.


    }
}
```
