using ServiceBoxRandom;

const int minServicesCount = 10;
const int maxServicesCount = 16;
const int maxServiceTime = 241; // min
const int maxPerson = 2;
const string directory = "Data";

var servicesPath = Path.Combine(directory, "services.csv");
var boxesPath = Path.Combine(directory, "boxes.csv");
var resultPath = Path.Combine(directory, "result.csv");

var services = CsvExtensions.GetRecords<Service>(servicesPath)
    .Where(s => s.Person < maxPerson)
    .ToArray();
var boxes = CsvExtensions.GetRecords<ServiceBox>(boxesPath);
var rand = new Random();

AddSingleServices(boxes, services);

var combinations = AddCombinedServices();
AddSingleServices(boxes, combinations);

var allBoxes = CreateRandomSets(boxes);
CsvExtensions.WriteBoxes(allBoxes, resultPath);

// Добавить все комбинированные сервисы 1+2(+3)
Service[] AddCombinedServices()
{
    var serviceCategory = services
        .GroupBy(s => s.Name)
        .ToDictionary(s => s.Key, s => s.ToArray());
    var allCombinations = new List<Service>();
    foreach (var (firstServiceName, firstServices) in serviceCategory)
    {
        foreach (var (secondServiceName, secondServices) in serviceCategory
                     .Where(c => c.Key != firstServiceName))
        {
            var newService = AddCombinedService(firstServices.First(), secondServices);

            foreach (var (_, thirdServices) in serviceCategory
                         .Where(c => c.Key != firstServiceName && c.Key != secondServiceName))
            {
                newService = AddCombinedService(newService, thirdServices);
            }
        }
    }

    return allCombinations.ToArray();

    Service AddCombinedService(Service service, Service[] otherServices)
    {
        var selected = rand.Next(0, otherServices.Length);
        var newService = CreateNewService(service, otherServices[selected]);

        // Проверка на максимально время процедуры
        if (newService.Time < maxServiceTime)
        {
            allCombinations.Add(newService);   
        }

        return newService;
    }
    
    Service CreateNewService(Service left, Service right)
    {
       return new Service
        {
            Id = $"{left.Id}+{right.Id}",
            Name = $"{left.Name}+{right.Name}",
            Person = left.Person,
            Time = left.Time + right.Time,
            AgentPrice = left.AgentPrice + right.AgentPrice
        };
    }
}

// Наполнить коробки одиночными сервисами
void AddSingleServices(ServiceBox[] inputBoxes, Service[] inputServices)
{
    foreach (var serviceBox in inputBoxes)
    {
        var suitableServices = inputServices
            .Where(s => s.AgentPrice <= serviceBox.MaxPrice && s.AgentPrice >= serviceBox.MinPrice)
            .ToArray();

        foreach (var newService in suitableServices)
        {
            var service = newService;
            if (serviceBox.Services.Any(s => s.Name == service.Name))
            {
                continue;
            }

            serviceBox.Services.Add(newService);
        }
    }
}

// Создает случайные наборы коробок
ServiceBox[] CreateRandomSets(ServiceBox[] inputBoxes)
{
    var boxesByName = inputBoxes.ToDictionary(b => b.Name, _ => new List<ServiceBox>());
    foreach (var box in inputBoxes)
    {
        bool hasUniqueSets;
        do
        {
            var newBox = box.Clone();
            var size = rand.Next(minServicesCount, maxServicesCount);
            var duplicates = boxesByName[box.Name].SelectMany(b => b.Services).Select(s => s.Id).ToArray();
            var newServices = box.Services.OrderBy(s => duplicates.Contains(s.Id))
                .ThenBy(_ => rand.Next())
                .Take(size)
                .ToList();
            newBox.Services = newServices;
            boxesByName[box.Name].Add(newBox);
            hasUniqueSets = box.Services.Count(s => !duplicates.Contains(s.Id)) >= 8;
        } while (hasUniqueSets);
    }

    return boxesByName.Values.SelectMany(b => b).ToArray();
}
