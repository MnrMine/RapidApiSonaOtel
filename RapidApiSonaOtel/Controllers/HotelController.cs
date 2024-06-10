using Microsoft.AspNetCore.Mvc;
using RapidApiSonaOtel.Models;
using Newtonsoft.Json;

namespace RapidApiSonaOtel.Controllers
{
    public class HotelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult HotelSearch()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> HotelSearch(HotelSearchViewModel model)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchDestination?query={model.cityName}"),
                Headers =
    {
        { "X-RapidAPI-Key", "d872a0b501mshaccbf8beb278337p1a2e61jsn7b1fbeaa3a51" },
        { "X-RapidAPI-Host", "booking-com15.p.rapidapi.com" },
    },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<CityIdViewModel>(body);
                var cityId = values.data[0].dest_id;
                var getSearch = new HotelSearchViewModel
                {
                    destID = cityId,
                    cityName = model.cityName,
                    arrivalDate = model.arrivalDate,
                    departureDate = model.departureDate,
                    //adultCount = model.adultCount,
                    //roomCount = model.roomCount
                };
                return RedirectToAction("HotelList", getSearch);
            }

        }

        public async Task<IActionResult>HotelList(HotelSearchViewModel model)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels?dest_id={model.destID}&search_type=CITY&arrival_date={model.arrivalDate.ToString("yyyy-MM-dd")}&departure_date={model.departureDate.ToString("yyyy-MM-dd")}&adults=2&room_qty=1&page_number=1&languagecode=en-us&currency_code=EUR"),
                Headers =
    {
        { "X-RapidAPI-Key", "d872a0b501mshaccbf8beb278337p1a2e61jsn7b1fbeaa3a51" },
        { "X-RapidAPI-Host", "booking-com15.p.rapidapi.com" },
    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<HotelListViewModel>(body);
                TempData["Photo"] = values.data.hotels[0].property.photoUrls[0].Replace("square60", "square480");
                return View(values.data.hotels.ToList());
            }
        }
        public IActionResult HotelDetail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> HotelDetail(string hotelID, string arrivalDate, string departureDate)
        {

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://booking-com15.p.rapidapi.com/api/v1/hotels/getHotelDetails?hotel_id={hotelID}&arrival_date={arrivalDate}&departure_date={departureDate}&languagecode=en-us&currency_code=EUR"),
                Headers = {
                  {
                     "X-RapidAPI-Key",
                     "d872a0b501mshaccbf8beb278337p1a2e61jsn7b1fbeaa3a51"
                  },
                  {
                     "X-RapidAPI-Host",
                     "booking-com15.p.rapidapi.com"
                  },
               },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<HotelDetailViewModel>(body);
                return View(value);
            }
        }
    }
}
