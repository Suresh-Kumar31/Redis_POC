using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedisBasicOperation
{
    class Program
    {
        static void Main(string[] args)
        {
            var expiry=new DateTimeOffset((DateTime.Now).AddSeconds(120));
            ICacheClient client = new StackExchangeRedisCacheClient(ConnectionMultiplexer.Connect("localhost"), new NewtonsoftSerializer());
            client.Add("key101", "Value", TimeSpan.FromSeconds(100));
            client.Add("key102", "Value", expiry);

            Console.WriteLine(client.Exists("key101"));
            Console.WriteLine(client.Exists("key106"));

            client.Replace("key101","New value");

            Console.WriteLine(client.Get<string>("key101"));
            client.Remove("key101");
            Console.Read();


            foreach (var item in client.GetInfo())
            {
                Console.WriteLine(item.Key + " " + item.Value);
            }


            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase(0);
            string value = "Data";

            Console.WriteLine("Setting 'Key1=Data' whith TTL=10sec");
            db.StringSet("Key1", value, TimeSpan.FromSeconds(10));
            Console.WriteLine("\nTTL:" + db.KeyTimeToLive("Key1"));

            Console.WriteLine("\nSleeping For 5sec...");
            Thread.Sleep(5000);
            Console.WriteLine("\n\nTTL:" + db.KeyTimeToLive("Key1"));

            Console.WriteLine("\nResetting TTL=10sec");
            db.KeyExpire("Key1", TimeSpan.FromSeconds(10));
            Console.WriteLine("\nTTL:" + db.KeyTimeToLive("Key1"));


            var time = DateTime.Now;
            string res; int i = 0;
            do
            {
                res = db.StringGet("Key1");
                if (res != null)
                    Console.WriteLine(DateTime.Now + "      " + res + " ->" + ++i + "th Sec");
                else
                    Console.WriteLine(DateTime.Now + "      " + "No value in Cache" + " ->" + ++i + "th Sec");

                Thread.Sleep(1000);
            } while (DateTime.Now < time.AddSeconds(11));

            Console.ReadKey();
        }
    }
}
