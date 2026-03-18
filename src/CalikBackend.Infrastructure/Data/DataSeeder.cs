using CalikBackend.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CalikBackend.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { "Admin", "Seller", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var config = services.GetRequiredService<IConfiguration>();
        var adminEmail = config["AdminSeed:Email"] ?? "admin@calik.com";
        var adminPassword = config["AdminSeed:Password"] ?? "System.InvalidOperationException: The LINQ expression 'DbSet<InvoiceItem>()\n    .GroupBy(i => new { \n        ProductId = i.ProductId, \n        ProductName = i.ProductName, \n        Unit = i.Unit\n     })\n    .Select(g => new TopProduct(\n        g.Key.ProductId, \n        g.Key.ProductName, \n        g.Key.Unit, \n        g\n            .AsQueryable()\n            .Sum(e => e.Quantity)\n    ))\n    .OrderByDescending(e0 => e0.TotalSold)' could not be translated. Either rewrite the query in a form that can be translated, or switch to client evaluation explicitly by inserting a call to 'AsEnumerable', 'AsAsyncEnumerable', 'ToList', or 'ToListAsync'. See https://go.microsoft.com/fwlink/?linkid=2101038 for more information.\n   at Microsoft.EntityFrameworkCore.Query.QueryableMethodTranslatingExpressionVisitor.Translate(Expression expression)\n   at Microsoft.EntityFrameworkCore.Query.QueryCompilationContext.CreateQueryExecutorExpression[TResult](Expression query)\n   at Microsoft.EntityFrameworkCore.Query.QueryCompilationContext.CreateQueryExecutor[TResult](Expression query)\n   at Microsoft.EntityFrameworkCore.Storage.Database.CompileQuery[TResult](Expression query, Boolean async)\n   at Microsoft.EntityFrameworkCore.Query.Internal.QueryCompiler.CompileQueryCore[TResult](IDatabase database, Expression query, IModel model, Boolean async)\n   at Microsoft.EntityFrameworkCore.Query.Internal.QueryCompiler.<>c__DisplayClass11_0`1.<ExecuteCore>b__0()\n   at Microsoft.EntityFrameworkCore.Query.Internal.CompiledQueryCache.GetOrAddQuery[TResult](Object cacheKey, Func`1 compiler)\n   at Microsoft.EntityFrameworkCore.Query.Internal.QueryCompiler.ExecuteCore[TResult](Expression query, Boolean async, CancellationToken cancellationToken)\n   at Microsoft.EntityFrameworkCore.Query.Internal.QueryCompiler.ExecuteAsync[TResult](Expression query, CancellationToken cancellationToken)\n   at Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider.ExecuteAsync[TResult](Expression expression, CancellationToken cancellationToken)\n   at Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable`1.GetAsyncEnumerator(CancellationToken cancellationToken)\n   at System.Runtime.CompilerServices.ConfiguredCancelableAsyncEnumerable`1.GetAsyncEnumerator()\n   at Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync[TSource](IQueryable`1 source, CancellationToken cancellationToken)\n   at CalikBackend.API.Controllers.AdminController.Dashboard() in /Users/gurkanmaral/Desktop/calik-backend/src/CalikBackend.API/Controllers/AdminController.cs:line 67\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ActionMethodExecutor.TaskOfIActionResultExecutor.Execute(ActionContext actionContext, IActionResultTypeMapper mapper, ObjectMethodExecutor executor, Object controller, Object[] arguments)\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeActionMethodAsync>g__Logged|12_1(ControllerActionInvoker invoker)\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeNextActionFilterAsync>g__Awaited|10_0(ControllerActionInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Rethrow(ActionExecutedContextSealed context)\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeInnerFilterAsync>g__Awaited|13_0(ControllerActionInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeFilterPipelineAsync>g__Awaited|20_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(ResourceInvoker invoker)\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(ResourceInvoker invoker)\n   at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)\n   at Microsoft.AspNetCore.Authentication.AuthenticationMiddleware.Invoke(HttpContext context)\n   at Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIMiddleware.Invoke(HttpContext httpContext)\n   at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)\n   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddlewareImpl.Invoke(HttpContext context)\n\nHEADERS\n=======\nAccept: */*\nConnection: keep-alive\nHost: localhost:5239\nUser-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/144.0.0.0 Safari/537.36\nAccept-Encoding: gzip, deflate, br, zstd\nAccept-Language: en-US,en;q=0.9,tr;q=0.8\nAuthorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIyZWVkZjIxYy1jOGU5LTQyM2YtYjkwYS1mY2I5NjUyODEyMjMiLCJlbWFpbCI6ImFkbWluQGNhbGlrLmNvbSIsImp0aSI6IjQyMmU5NGRlLWI5M2EtNDYzYS1iZGI3LWQxNDExZGM4ODNhZiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJhZG1pbkBjYWxpay5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImV4cCI6MTc3MzQ5MDA5NCwiaXNzIjoiQ2FsaWtCYWNrZW5kIiwiYXVkIjoiQ2FsaWtCYWNrZW5kIn0.0XMZWYXigYbBsfGMZVJgCoTuRQu7FaIe8_zEFB5EULw\nCookie: authjs.csrf-token=3161b0b6259994adf19f59033ad7f720ed3a751a95f9eceb3690925446775e9c%7C68b0fcff8e14f78b0858a80a722447c0b08bde72aacc35e32dd69439f7e90c1f; authjs.callback-url=http%3A%2F%2Flocalhost%3A3001%2Fauth; _ga=GA1.1.2072078583.1758114248; jwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MSwiaWF0IjoxNzYxNjQ4NDI4LCJleHAiOjE3NjQyNDA0Mjh9.CuAnaGUipK2x9_EPcpcSf_Mr_dplqzmn07DCBHyjBg4; _clck=k6l8db%5E2%5Eg1c%5E0%5E2156; __next_hmr_refresh_hash__=39; _gcl_au=1.1.1782087174.1771945165; _ga_6P2DNJPDTL=GS2.1.s1773061943$o44$g0$t1773062225$j60$l0$h0\nReferer: http://localhost:5239/swagger/index.html\nsec-ch-ua-platform: \"macOS\"\nsec-ch-ua: \"Not(A:Brand\";v=\"8\", \"Chromium\";v=\"144\", \"Google Chrome\";v=\"144\"\nsec-ch-ua-mobile: ?0\nSec-Fetch-Site: same-origin\nSec-Fetch-Mode: cors\nSec-Fetch-Dest: empty";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, FirstName = "Admin", LastName = "User" };
            await userManager.CreateAsync(admin, adminPassword);
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        var db = services.GetRequiredService<AppDbContext>();
        if (!await db.ProductCategories.AnyAsync())
        {
            db.ProductCategories.AddRange(
                new ProductCategory { Name = "Batteries",           Description = "Rechargeable and primary batteries", ImageUrl = "https://placehold.co/600x400/1a1a2e/ffffff?text=Batteries" },
                new ProductCategory { Name = "Chargers",            Description = "Battery chargers and adapters",      ImageUrl = "https://placehold.co/600x400/16213e/ffffff?text=Chargers" },
                new ProductCategory { Name = "Inverters",           Description = "DC to AC power inverters",          ImageUrl = "https://placehold.co/600x400/0f3460/ffffff?text=Inverters" },
                new ProductCategory { Name = "Solar Equipment",     Description = "Solar panels and accessories",      ImageUrl = "https://placehold.co/600x400/e94560/ffffff?text=Solar" },
                new ProductCategory { Name = "Cables & Connectors", Description = "Power and signal cables",           ImageUrl = "https://placehold.co/600x400/533483/ffffff?text=Cables" },
                new ProductCategory { Name = "LED Lighting",        Description = "Energy-efficient LED products",     ImageUrl = "https://placehold.co/600x400/f5a623/ffffff?text=LED" },
                new ProductCategory { Name = "UPS Systems",         Description = "Uninterruptible power supplies",    ImageUrl = "https://placehold.co/600x400/2c3e50/ffffff?text=UPS" },
                new ProductCategory { Name = "Power Tools",         Description = "Cordless and corded power tools",   ImageUrl = "https://placehold.co/600x400/c0392b/ffffff?text=Tools" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Products.AnyAsync())
        {
            var batteries  = await db.ProductCategories.FirstAsync(c => c.Name == "Batteries");
            var chargers   = await db.ProductCategories.FirstAsync(c => c.Name == "Chargers");
            var inverters  = await db.ProductCategories.FirstAsync(c => c.Name == "Inverters");
            var solar      = await db.ProductCategories.FirstAsync(c => c.Name == "Solar Equipment");
            var cables     = await db.ProductCategories.FirstAsync(c => c.Name == "Cables & Connectors");
            var led        = await db.ProductCategories.FirstAsync(c => c.Name == "LED Lighting");
            var ups        = await db.ProductCategories.FirstAsync(c => c.Name == "UPS Systems");
            var powerTools = await db.ProductCategories.FirstAsync(c => c.Name == "Power Tools");

            db.Products.AddRange(
                // Batteries
                new Product { Name = "Calik LiFePO4 100Ah",       Brand = "Calik", Model = "LFP-100",  Price = 4200,  Stock = 25,  Unit = "adet", CategoryId = batteries.Id,  Description = "12V 100Ah lithium iron phosphate battery", ImageUrl = "https://placehold.co/600x400/1a1a2e/ffffff?text=LiFePO4+100Ah" },
                new Product { Name = "Calik AGM 75Ah",            Brand = "Calik", Model = "AGM-75",   Price = 1850,  Stock = 40,  Unit = "adet", CategoryId = batteries.Id,  Description = "12V 75Ah AGM deep cycle battery",            ImageUrl = "https://placehold.co/600x400/1a1a2e/ffffff?text=AGM+75Ah" },
                new Product { Name = "Calik Gel 200Ah",           Brand = "Calik", Model = "GEL-200",  Price = 5600,  Stock = 15,  Unit = "adet", CategoryId = batteries.Id,  Description = "12V 200Ah gel battery for solar systems",    ImageUrl = "https://placehold.co/600x400/1a1a2e/ffffff?text=Gel+200Ah" },

                // Chargers
                new Product { Name = "Calik Smart Charger 20A",   Brand = "Calik", Model = "SC-20A",   Price = 890,   Stock = 30,  Unit = "adet", CategoryId = chargers.Id,   Description = "12/24V 20A intelligent battery charger",     ImageUrl = "https://placehold.co/600x400/16213e/ffffff?text=Smart+Charger+20A" },
                new Product { Name = "Calik MPPT Charger 40A",    Brand = "Calik", Model = "MPPT-40",  Price = 2100,  Stock = 18,  Unit = "adet", CategoryId = chargers.Id,   Description = "Solar MPPT charge controller 40A",           ImageUrl = "https://placehold.co/600x400/16213e/ffffff?text=MPPT+40A" },
                new Product { Name = "Calik Trickle Charger 5A",  Brand = "Calik", Model = "TC-5A",    Price = 320,   Stock = 50,  Unit = "adet", CategoryId = chargers.Id,   Description = "Maintenance trickle charger 5A",             ImageUrl = "https://placehold.co/600x400/16213e/ffffff?text=Trickle+5A" },

                // Inverters
                new Product { Name = "Calik Pure Sine 1000W",     Brand = "Calik", Model = "PSI-1000", Price = 3200,  Stock = 12,  Unit = "adet", CategoryId = inverters.Id,  Description = "12V pure sine wave inverter 1000W",          ImageUrl = "https://placehold.co/600x400/0f3460/ffffff?text=Pure+Sine+1000W" },
                new Product { Name = "Calik Pure Sine 2000W",     Brand = "Calik", Model = "PSI-2000", Price = 5800,  Stock = 8,   Unit = "adet", CategoryId = inverters.Id,  Description = "24V pure sine wave inverter 2000W",          ImageUrl = "https://placehold.co/600x400/0f3460/ffffff?text=Pure+Sine+2000W" },
                new Product { Name = "Calik Modified Sine 600W",  Brand = "Calik", Model = "MSI-600",  Price = 1400,  Stock = 20,  Unit = "adet", CategoryId = inverters.Id,  Description = "12V modified sine wave inverter 600W",       ImageUrl = "https://placehold.co/600x400/0f3460/ffffff?text=Modified+600W" },

                // Solar Equipment
                new Product { Name = "Mono Solar Panel 250W",     Brand = "Calik", Model = "SP-250M",  Price = 2750,  Stock = 60,  Unit = "adet", CategoryId = solar.Id,      Description = "Monocrystalline solar panel 250W 24V",       ImageUrl = "https://placehold.co/600x400/e94560/ffffff?text=Mono+250W" },
                new Product { Name = "Poly Solar Panel 150W",     Brand = "Calik", Model = "SP-150P",  Price = 1500,  Stock = 45,  Unit = "adet", CategoryId = solar.Id,      Description = "Polycrystalline solar panel 150W 12V",       ImageUrl = "https://placehold.co/600x400/e94560/ffffff?text=Poly+150W" },
                new Product { Name = "Solar Panel Mount Kit",     Brand = "Calik", Model = "MK-01",    Price = 480,   Stock = 35,  Unit = "set",  CategoryId = solar.Id,      Description = "Adjustable roof mounting kit for 2 panels",  ImageUrl = "https://placehold.co/600x400/e94560/ffffff?text=Mount+Kit" },

                // Cables & Connectors
                new Product { Name = "Solar Cable 6mm² 10m",      Brand = "Calik", Model = "SC6-10",   Price = 220,   Stock = 80,  Unit = "adet", CategoryId = cables.Id,     Description = "UV-resistant solar cable 6mm² red+black 10m",ImageUrl = "https://placehold.co/600x400/533483/ffffff?text=Solar+Cable+6mm" },
                new Product { Name = "MC4 Connector Pair",        Brand = "Calik", Model = "MC4-PR",   Price = 45,    Stock = 200, Unit = "çift", CategoryId = cables.Id,     Description = "Weatherproof MC4 solar connector pair",      ImageUrl = "https://placehold.co/600x400/533483/ffffff?text=MC4+Connector" },
                new Product { Name = "Battery Cable Set 25mm²",   Brand = "Calik", Model = "BCS-25",   Price = 185,   Stock = 60,  Unit = "set",  CategoryId = cables.Id,     Description = "25mm² battery interconnect cables with lugs", ImageUrl = "https://placehold.co/600x400/533483/ffffff?text=Battery+Cable" },

                // LED Lighting
                new Product { Name = "Calik LED Panel 48W",       Brand = "Calik", Model = "LP-48",    Price = 340,   Stock = 55,  Unit = "adet", CategoryId = led.Id,        Description = "Surface mount LED panel 48W 6500K",          ImageUrl = "https://placehold.co/600x400/f5a623/ffffff?text=LED+Panel+48W" },
                new Product { Name = "Calik LED Floodlight 100W", Brand = "Calik", Model = "FL-100",   Price = 620,   Stock = 30,  Unit = "adet", CategoryId = led.Id,        Description = "Outdoor LED floodlight 100W IP65",           ImageUrl = "https://placehold.co/600x400/f5a623/ffffff?text=Floodlight+100W" },
                new Product { Name = "Calik LED Strip 5m",        Brand = "Calik", Model = "LS-5M",    Price = 150,   Stock = 90,  Unit = "kutu", CategoryId = led.Id,        Description = "12V LED strip 5050 RGB 5m roll",             ImageUrl = "https://placehold.co/600x400/f5a623/ffffff?text=LED+Strip+5m" },

                // UPS Systems
                new Product { Name = "Calik UPS 1kVA Online",     Brand = "Calik", Model = "UPS-1K",   Price = 6500,  Stock = 10,  Unit = "adet", CategoryId = ups.Id,        Description = "Online double-conversion UPS 1kVA",          ImageUrl = "https://placehold.co/600x400/2c3e50/ffffff?text=UPS+1kVA" },
                new Product { Name = "Calik UPS 2kVA Online",     Brand = "Calik", Model = "UPS-2K",   Price = 11200, Stock = 6,   Unit = "adet", CategoryId = ups.Id,        Description = "Online double-conversion UPS 2kVA",          ImageUrl = "https://placehold.co/600x400/2c3e50/ffffff?text=UPS+2kVA" },

                // Power Tools
                new Product { Name = "Calik Drill 18V",           Brand = "Calik", Model = "CD-18",    Price = 1250,  Stock = 22,  Unit = "adet", CategoryId = powerTools.Id, Description = "Cordless drill/driver 18V with 2Ah battery", ImageUrl = "https://placehold.co/600x400/c0392b/ffffff?text=Drill+18V" },
                new Product { Name = "Calik Angle Grinder 125mm", Brand = "Calik", Model = "AG-125",   Price = 780,   Stock = 18,  Unit = "adet", CategoryId = powerTools.Id, Description = "850W angle grinder 125mm disc",              ImageUrl = "https://placehold.co/600x400/c0392b/ffffff?text=Grinder+125mm" }
            );
            await db.SaveChangesAsync();
        }

        // Patch any existing records that are missing an ImageUrl
        var productsWithNoImage = await db.Products.Where(p => p.ImageUrl == null).ToListAsync();
        foreach (var p in productsWithNoImage)
            p.ImageUrl = "https://placehold.co/600x400/cccccc/333333?text=" + Uri.EscapeDataString(p.Name);
        if (productsWithNoImage.Count > 0) await db.SaveChangesAsync();

        var categoriesWithNoImage = await db.ProductCategories.Where(c => c.ImageUrl == null).ToListAsync();
        foreach (var c in categoriesWithNoImage)
            c.ImageUrl = "https://placehold.co/600x400/cccccc/333333?text=" + Uri.EscapeDataString(c.Name);
        if (categoriesWithNoImage.Count > 0) await db.SaveChangesAsync();

        if (!await db.Customers.AnyAsync())
        {
            db.Customers.AddRange(
                new Customer { Name = "Ahmet Yılmaz",    PhoneNumber = "5321234567", CountryCode = "+90", Email = "ahmet.yilmaz@gmail.com",    Address = "Atatürk Cad. No:12",   City = "İstanbul",  District = "Kadıköy",    Balance = 3500 },
                new Customer { Name = "Mehmet Kaya",     PhoneNumber = "5339876543", CountryCode = "+90", Email = "mehmet.kaya@hotmail.com",    Address = "Cumhuriyet Sok. No:7",  City = "Ankara",    District = "Çankaya",    Balance = -800 },
                new Customer { Name = "Fatma Demir",     PhoneNumber = "5355551234", CountryCode = "+90", Email = "fatma.demir@gmail.com",      Address = "İnönü Cad. No:45",     City = "İzmir",     District = "Bornova",    Balance = 0 },
                new Customer { Name = "Ali Şahin",       PhoneNumber = "5362223344", CountryCode = "+90", Email = "ali.sahin@outlook.com",      Address = "Millet Cad. No:3",     City = "Bursa",     District = "Osmangazi",  Balance = 12750 },
                new Customer { Name = "Ayşe Çelik",      PhoneNumber = "5374445566", CountryCode = "+90", Email = "ayse.celik@gmail.com",       Address = "Barbaros Blv. No:88",  City = "İstanbul",  District = "Beşiktaş",  Balance = -2200 },
                new Customer { Name = "Hüseyin Arslan",  PhoneNumber = "5386667788", CountryCode = "+90", Email = "huseyin.arslan@gmail.com",   Address = "Fevzi Paşa Cad. No:21",City = "Ankara",    District = "Keçiören",   Balance = 5400 },
                new Customer { Name = "Zeynep Koç",      PhoneNumber = "5398889900", CountryCode = "+90", Email = "zeynep.koc@hotmail.com",     Address = "Alsancak Mah. No:55",  City = "İzmir",     District = "Konak",      Balance = 1100 },
                new Customer { Name = "Mustafa Yıldız",  PhoneNumber = "5311112233", CountryCode = "+90", Email = "mustafa.yildiz@gmail.com",   Address = "Yeni Mah. No:9",       City = "Antalya",   District = "Muratpaşa",  Balance = 0 },
                new Customer { Name = "Elif Özdemir",    PhoneNumber = "5323334455", CountryCode = "+90", Email = "elif.ozdemir@outlook.com",   Address = "Güneş Sok. No:14",     City = "Konya",     District = "Selçuklu",   Balance = 7800 },
                new Customer { Name = "İbrahim Doğan",   PhoneNumber = "5335556677", CountryCode = "+90", Email = "ibrahim.dogan@gmail.com",    Address = "Lale Cad. No:32",      City = "Adana",     District = "Seyhan",     Balance = -450 }
            );
            await db.SaveChangesAsync();
        }
    }
}
