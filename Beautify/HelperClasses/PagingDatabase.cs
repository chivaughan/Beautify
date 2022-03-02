using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace Beautify
{
    public static class PagingDatabase
    {
        #region Services Pagination without status

        public static IEnumerable<Service> GetServicesData(string salonEmail, string category, int pageSize, int index)
        {
            IEnumerable<Service> serviceSource = SearchServices(salonEmail, category);
            int skipUpto = ((index - 1) * pageSize);

            IEnumerable<Service> searchResult = serviceSource.Skip(skipUpto).Take(pageSize);
            return searchResult;

        }

        public static int GetServicesCount(string salonEmail, string category)
        {
            List<Service> services = GetServices(salonEmail);
            int servicesCount = 0;

            if (String.IsNullOrEmpty(category) || category.Equals("All"))
            {
                servicesCount = services.Count;
            }
            else
            {
                List<Service> selectedServices = services.Where(r => r.serviceCategory == category).ToList();
                servicesCount = selectedServices.Count;
            }

            return servicesCount;
        }

        private static IEnumerable<Service> SearchServices(string salonEmail, string category)
        {
            List<Service> services = GetServices(salonEmail);
            if (String.IsNullOrEmpty(category) || category.Equals("All"))
            {
                return services;
            }

            return services.Where(r => r.serviceCategory == category);
        }

        private static List<Service> GetServices(string salonEmail)
        {
            List<Service> services = new List<Service>();

            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString;
            selectString = @"select * from ServicePricing WHERE SalonEmail = @SalonEmail ORDER BY ServiceID DESC";

            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the SalonEmail parameter
            da.SelectCommand.Parameters.AddWithValue("@SalonEmail", salonEmail);

            dt = new DataTable();
            da.Fill(dt);

            // Add all services found
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Service serv = new Service();
                serv.serviceID = int.Parse(dt.Rows[i]["ServiceID"].ToString());
                serv.salonEmail = salonEmail;
                serv.serviceName = dt.Rows[i]["ServiceName"].ToString();
                serv.serviceCategory = dt.Rows[i]["ServiceCategory"].ToString();
                serv.shortDescription = dt.Rows[i]["ShortDescription"].ToString();
                serv.fullDescription = dt.Rows[i]["FullDescription"].ToString();
                serv.imageUrl = dt.Rows[i]["ImageUrl"].ToString();
                serv.serviceCost = double.Parse(dt.Rows[i]["ServiceCost"].ToString());
                serv.serviceStatus = dt.Rows[i]["ServiceStatus"].ToString();
                serv.dateAdded = dt.Rows[i]["DateAdded"].ToString();
                serv.dateUpdated = dt.Rows[i]["DateUpdated"].ToString();

                // Add each service to the list
                services.Add(serv);
            }

            da.Dispose();
            dt.Clear();
            conn.Close();

            // Return the services
            return services;
        }

        #endregion

        #region Reviews Pagination

        public static IEnumerable<Review> GetReviewsData(string salonEmail, int pageSize, int index)
        {
            IEnumerable<Review> reviewsSource = SearchReviews(salonEmail);
            int skipUpto = ((index - 1) * pageSize);

            IEnumerable<Review> searchResult = reviewsSource.Skip(skipUpto).Take(pageSize);
            return searchResult;

        }

        public static int GetReviewsCount(string salonEmail)
        {
            List<Review> reviews = GetReviews(salonEmail);
            int reviewsCount = 0;

            reviewsCount = reviews.Count;
            
            return reviewsCount;
        }

        private static IEnumerable<Review> SearchReviews(string salonEmail)
        {
            List<Review> reviews = GetReviews(salonEmail);
            return reviews;
            
        }

        private static List<Review> GetReviews(string salonEmail)
        {
            List<Review> reviews = new List<Review>();

            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString;
            selectString = @"select Bookings.ClientName, RatingsAndReviews.* from RatingsAndReviews INNER JOIN Bookings on RatingsAndReviews.BookingID = Bookings.BookingID WHERE RatingsAndReviews.SalonEmail = @SalonEmail ORDER BY SN DESC";

            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the SalonEmail parameter
            da.SelectCommand.Parameters.AddWithValue("@SalonEmail", salonEmail);

            dt = new DataTable();
            da.Fill(dt);

            // Add all reviews found
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Review rev = new Review();
                rev.bookingID = dt.Rows[i]["BookingID"].ToString();
                rev.clientName = dt.Rows[i]["ClientName"].ToString();
                rev.salonEmail = dt.Rows[i]["SalonEmail"].ToString();
                rev.rating = double.Parse(dt.Rows[i]["Rating"].ToString());
                rev.comment = dt.Rows[i]["Comment"].ToString();
                rev.reviewDate = dt.Rows[i]["ReviewDate"].ToString();
                rev.numericalReviewDate = dt.Rows[i]["NumericalReviewDate"].ToString();
                
                // Add each review to the list
                reviews.Add(rev);
            }

            da.Dispose();
            dt.Clear();
            conn.Close();

            // Return the reviews
            return reviews;
        }

        #endregion

        #region Salons Pagination

        public static IEnumerable<Salon> GetSalonsData(string city, string locationInCity, string orderBy, int pageSize, int index)
        {
            IEnumerable<Salon> salonsSource = SearchSalons(city, locationInCity, orderBy);
            int skipUpto = ((index - 1) * pageSize);

            IEnumerable<Salon> searchResult = salonsSource.Skip(skipUpto).Take(pageSize);
            return searchResult;

        }

        public static int GetSalonsCount(string city, string locationInCity)
        {
            List<Salon> salons = GetSalons(city, locationInCity);
            int salonsCount = 0;
            salonsCount = salons.Count;
            
            return salonsCount;
        }

        private static IEnumerable<Salon> SearchSalons(string city, string locationInCity, string orderBy)
        {
            List<Salon> salons = GetSalons(city, locationInCity);
            
            // Switch the orderBy to decide how to order the salons list
            switch (orderBy)
            {
                case "SalonName ASC":
                    return salons.OrderBy(sal => sal.salonName);
                case "SalonName DESC":
                    return salons.OrderByDescending(sal => sal.salonName);
                case "AverageRating ASC":
                    return salons.OrderBy(sal => sal.averageRating);
                case "AverageRating DESC":
                    return salons.OrderByDescending(sal => sal.averageRating);
                case "NumberOfBookings ASC":
                    return salons.OrderBy(sal => sal.numberOfBookings);
                case "NumberOfBookings DESC":
                    return salons.OrderByDescending(sal => sal.numberOfBookings);
                default:
                    return salons.OrderBy(sal => sal.salonName);
            }
            
        }

        private static List<Salon> GetSalons(string city, string locationInCity)
        {
            List<Salon> salons = new List<Salon>();

            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString;
            // Let us fetch salons in the specified city. Note that only salons with ACTIVE rent will be returned
            selectString = @";with SalonInfo as" +
                " (select Salons.*, isnull(sum(RatingsAndReviews.Rating),0) as TotalRating, COUNT(RatingsAndReviews.Rating) as NumberOfVotes from " +
                "salons left JOIN RatingsAndReviews ON RatingsAndReviews.SalonEmail = Salons.Email where Salons.Locations like @Locations AND Salons.LocationsInCities like @LocationsInCities " +
                "AND Salons.RentStatus = 'ACTIVE' group by salons.Username, salons.BookingStatus, salons.DateRegistered, salons.Email, salons.ImageUrl," +
                " salons.Locations, salons.NumericalDateRegistered, salons.RentStatus, salons.SalonName, RatingsAndReviews.Rating, salons.About, salons.OpeningTime, salons.ClosingTime, salons.DisabledDays, salons.BankName, salons.AccountName, salons.AccountNumber, salons.PhoneNumber, salons.LocationsInCities) select SalonInfo.Username,  " +
                "SalonInfo.Email, SalonInfo.SalonName, SalonInfo.ImageUrl, SalonInfo.Locations, " +
                "sum(totalrating) as TotalRating, sum(NumberOfVotes) as NumberOfVotes from " +
                "SalonInfo group by SalonInfo.Username, SalonInfo.Email, SalonInfo.ImageUrl, SalonInfo.Locations, " +
                "SalonInfo.SalonName";

            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);

            // Add the Locations parameter
            da.SelectCommand.Parameters.AddWithValue("@Locations", "%" + city + "%");

            // Add the apropriate Location in City parameter
            if (locationInCity.Equals("All"))
            {
                // Here we are searching for all locations
                da.SelectCommand.Parameters.AddWithValue("@LocationsInCities", "%%");
            }
            else
            {
                // Here we are searching for the specified location only
                da.SelectCommand.Parameters.AddWithValue("@LocationsInCities", "%" + locationInCity + "%");
            }

            dt = new DataTable();
            da.Fill(dt);

            // Add all salons found
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Salon sal = new Salon();
                sal.username = dt.Rows[i]["Username"].ToString();
                sal.email = dt.Rows[i]["Email"].ToString();
                sal.salonName = dt.Rows[i]["SalonName"].ToString();
                sal.imageUrl = dt.Rows[i]["ImageUrl"].ToString();
                sal.locations = dt.Rows[i]["Locations"].ToString();
                sal.totalRating = double.Parse(dt.Rows[i]["TotalRating"].ToString());
                sal.numberOfVotes = int.Parse(dt.Rows[i]["NumberOfVotes"].ToString());
                // set the default average rating for the current salon
                sal.averageRating = 0;
                // We should only recompute the average if the NumberOfVotes != 0 to avoid a division by 0 error
                if (sal.numberOfVotes != 0)
                {
                    sal.averageRating = sal.totalRating / sal.numberOfVotes;
                }
                
                sal.numberOfBookings = GetNumberOfBookings(sal.email);

                // Add each salon to the list
                salons.Add(sal);
            }

            da.Dispose();
            dt.Clear();
            conn.Close();

            // Return the salons
            return salons;
        }


        /// <summary>
        /// Returns the number of ATTENDED bookings for a salon
        /// </summary>
        /// <param name="salonEmail">The salon's email adrress</param>
        /// <returns>The number of ATTENDED bookings for a salon</returns>
        private static int GetNumberOfBookings(string salonEmail)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT COUNT(BookingID) as NumberOfBookings from Bookings WHERE SalonEmail = @SalonEmail AND BookingStatus = 'ATTENDED'";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            da.SelectCommand.Parameters.AddWithValue("@SalonEmail", salonEmail);
            dt = new DataTable();
            da.Fill(dt);
            int numberOfBookings = 0;
            if (dt.Rows.Count != 0)
            {
                numberOfBookings = int.Parse(dt.Rows[0]["NumberOfBookings"].ToString());
            }
            da.Dispose();
            dt.Clear();
            conn.Close();
            // Return the number of bookings
            return numberOfBookings;
        }

        #endregion


        #region Services Pagination with Status
        public static IEnumerable<Service> GetServicesDataWithStatus(string salonEmail, string category, string status, int pageSize, int index)
        {
            IEnumerable<Service> serviceSource = SearchServicesWithStatus(salonEmail, category, status);
            int skipUpto = ((index - 1) * pageSize);

            IEnumerable<Service> searchResult = serviceSource.Skip(skipUpto).Take(pageSize);
            return searchResult;

        }

        public static int GetServicesCountWithStatus(string salonEmail, string category, string status)
        {
            List<Service> services = GetServicesWithStatus(salonEmail, status);
            int servicesCount = 0;

            if (String.IsNullOrEmpty(category) || category.Equals("All"))
            {
                servicesCount = services.Count;
            }
            else
            {
                List<Service> selectedServices = services.Where(r => r.serviceCategory == category).ToList();
                servicesCount = selectedServices.Count;
            }

            return servicesCount;
        }

        private static IEnumerable<Service> SearchServicesWithStatus(string salonEmail, string category, string status)
        {
            List<Service> services = GetServicesWithStatus(salonEmail, status);
            if (String.IsNullOrEmpty(category) || category.Equals("All"))
            {
                return services;
            }

            return services.Where(r => r.serviceCategory == category);
        }

        private static List<Service> GetServicesWithStatus(string salonEmail, string status)
        {
            List<Service> services = new List<Service>();

            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString;
            selectString = @"select * from ServicePricing WHERE SalonEmail = @SalonEmail AND ServiceStatus = @ServiceStatus ORDER BY ServiceID DESC";

            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the parameters
            da.SelectCommand.Parameters.AddWithValue("@SalonEmail", salonEmail);
            da.SelectCommand.Parameters.AddWithValue("@ServiceStatus", Convert.ToBoolean(status));

            dt = new DataTable();
            da.Fill(dt);

            // Add all services found
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Service serv = new Service();
                serv.serviceID = int.Parse(dt.Rows[i]["ServiceID"].ToString());
                serv.salonEmail = salonEmail;
                serv.serviceName = dt.Rows[i]["ServiceName"].ToString();
                serv.serviceCategory = dt.Rows[i]["ServiceCategory"].ToString();
                serv.shortDescription = dt.Rows[i]["ShortDescription"].ToString();
                serv.fullDescription = dt.Rows[i]["FullDescription"].ToString();
                serv.imageUrl = dt.Rows[i]["ImageUrl"].ToString();
                serv.serviceCost = double.Parse(dt.Rows[i]["ServiceCost"].ToString());
                serv.serviceStatus = dt.Rows[i]["ServiceStatus"].ToString();
                serv.dateAdded = dt.Rows[i]["DateAdded"].ToString();
                serv.dateUpdated = dt.Rows[i]["DateUpdated"].ToString();

                // Add each service to the list
                services.Add(serv);
            }

            da.Dispose();
            dt.Clear();
            conn.Close();

            // Return the services
            return services;
        }

        
        #endregion

        #region Bookings Pagination

        public static IEnumerable<BookingInfo> GetBookingsData(string salonEmail, string status, int pageSize, int index)
        {
            IEnumerable<BookingInfo> bookingsSource = SearchBookings(salonEmail, status);
            int skipUpto = ((index - 1) * pageSize);

            IEnumerable<BookingInfo> searchResult = bookingsSource.Skip(skipUpto).Take(pageSize);
            return searchResult;

        }

        public static int GetBookingsCount(string salonEmail, string status)
        {
            List<BookingInfo> bookings = GetBookings(salonEmail);
            int bookingsCount = 0;

            if (String.IsNullOrEmpty(status) || status.Equals("All"))
            {
                bookingsCount = bookings.Count;
            }
            else
            {
                List<BookingInfo> selectedBookings = bookings.Where(r => r.status == status).ToList();
                bookingsCount = selectedBookings.Count;
            }

            return bookingsCount;
        }

        private static IEnumerable<BookingInfo> SearchBookings(string salonEmail, string status)
        {
            List<BookingInfo> bookings = GetBookings(salonEmail);
            if (String.IsNullOrEmpty(status) || status.Equals("All"))
            {
                return bookings;
            }

            return bookings.Where(r => r.status == status);
        }

        private static List<BookingInfo> GetBookings(string salonEmail)
        {
            List<BookingInfo> bookings = new List<BookingInfo>();

            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString;
            // Note that we are selecting only PAID bookings for this salon
            selectString = @"select BookingID, ClientName, SubTotal, BookingStatus, ClientChoiceDate from Bookings WHERE SalonEmail = @SalonEmail AND PaymentStatus = 'PAID' ORDER BY NumericalClientChoiceDate DESC";

            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the SalonEmail parameter
            da.SelectCommand.Parameters.AddWithValue("@SalonEmail", salonEmail);

            dt = new DataTable();
            da.Fill(dt);

            // Add all services found
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                BookingInfo book = new BookingInfo();
                book.bookingID = dt.Rows[i]["BookingID"].ToString();
                book.salonEmail = salonEmail;
                book.clientName = dt.Rows[i]["ClientName"].ToString();
                book.subTotal = double.Parse(dt.Rows[i]["SubTotal"].ToString());
                book.status = dt.Rows[i]["BookingStatus"].ToString();
                book.clientChoiceDate = dt.Rows[i]["ClientChoiceDate"].ToString();
                
                // Add each booking to the list
                bookings.Add(book);
            }

            da.Dispose();
            dt.Clear();
            conn.Close();

            // Return the services
            return bookings;
        }

        #endregion

        #region Portfolio Pagiation
        public static IEnumerable<Portfolio> GetPortfolioData(string salonEmail, string category, int pageSize, int index)
        {
            IEnumerable<Portfolio> portfolioSource = SearchPortfolio(salonEmail, category);
            int skipUpto = ((index - 1) * pageSize);

            IEnumerable<Portfolio> searchResult = portfolioSource.Skip(skipUpto).Take(pageSize);
            return searchResult;

        }

        public static int GetPortfolioCount(string salonEmail, string category)
        {
            List<Portfolio> portfolio = GetPortfolio(salonEmail);
            int portfolioCount = 0;

            if (String.IsNullOrEmpty(category) || category.Equals("All"))
            {
                portfolioCount = portfolio.Count;
            }
            else
            {
                List<Portfolio> selectedPortfolio = portfolio.Where(r => r.serviceCategory == category).ToList();
                portfolioCount = selectedPortfolio.Count;
            }

            return portfolioCount;
        }

        private static IEnumerable<Portfolio> SearchPortfolio(string salonEmail, string category)
        {
            List<Portfolio> portfolio = GetPortfolio(salonEmail);
            if (String.IsNullOrEmpty(category) || category.Equals("All"))
            {
                return portfolio;
            }

            return portfolio.Where(r => r.serviceCategory == category);
        }

        private static List<Portfolio> GetPortfolio(string salonEmail)
        {
            List<Portfolio> portfolio = new List<Portfolio>();

            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString;
            selectString = @"select * from Portfolio WHERE SalonEmail = @SalonEmail ORDER BY ID DESC";

            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the SalonEmail parameter
            da.SelectCommand.Parameters.AddWithValue("@SalonEmail", salonEmail);

            dt = new DataTable();
            da.Fill(dt);

            // Add all portfolio items found
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Portfolio port = new Portfolio();
                port.id = int.Parse(dt.Rows[i]["ID"].ToString());
                port.salonEmail = salonEmail;
                port.title = dt.Rows[i]["Title"].ToString();
                port.serviceCategory = dt.Rows[i]["ServiceCategory"].ToString();
                port.description = dt.Rows[i]["Description"].ToString();
                port.imageUrl = dt.Rows[i]["ImageUrl"].ToString();
                port.dateAdded = dt.Rows[i]["DateAdded"].ToString();
                port.dateUpdated = dt.Rows[i]["DateUpdated"].ToString();

                // Add each portfolio item to the list
                portfolio.Add(port);
            }

            da.Dispose();
            dt.Clear();
            conn.Close();

            // Return the services
            return portfolio;
        }
        #endregion


        #region Earnings Pagination

        public static IEnumerable<Earning> GetEarningsData(string salonEmail, string status, int pageSize, int index)
        {
            IEnumerable<Earning> earningsSource = SearchEarnings(salonEmail, status);
            int skipUpto = ((index - 1) * pageSize);

            IEnumerable<Earning> searchResult = earningsSource.Skip(skipUpto).Take(pageSize);
            return searchResult;

        }

        public static int GetEarningsCount(string salonEmail, string status)
        {
            List<Earning> earnings = GetEarnings(salonEmail);
            int earningsCount = 0;

            if (String.IsNullOrEmpty(status) || status.Equals("All"))
            {
                earningsCount = earnings.Count;
            }
            else
            {
                List<Earning> selectedEarnings = earnings.Where(r => r.earningPaymentStatus == status).ToList();
                earningsCount = selectedEarnings.Count;
            }

            return earningsCount;
        }

        private static IEnumerable<Earning> SearchEarnings(string salonEmail, string status)
        {
            List<Earning> earnings = GetEarnings(salonEmail);
            if (String.IsNullOrEmpty(status) || status.Equals("All"))
            {
                return earnings;
            }

            return earnings.Where(r => r.earningPaymentStatus == status);
        }

        private static List<Earning> GetEarnings(string salonEmail)
        {
            List<Earning> earnings = new List<Earning>();

            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString;
            // Note that we are selecting all earnings for this salon
            selectString = @"select * from SalonEarnings WHERE SalonEmail = @SalonEmail ORDER BY NumericalDatePaid DESC";

            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the SalonEmail parameter
            da.SelectCommand.Parameters.AddWithValue("@SalonEmail", salonEmail);

            dt = new DataTable();
            da.Fill(dt);

            // Add all earnings found
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Earning earn = new Earning();
                earn.bookingID = dt.Rows[i]["BookingID"].ToString();
                earn.clientName = dt.Rows[i]["ClientName"].ToString();
                earn.salonEmail = salonEmail;
                earn.serviceCost = double.Parse(dt.Rows[i]["ServiceCost"].ToString());
                earn.serviceCommissionPercent = int.Parse(dt.Rows[i]["ServiceCommissionPercent"].ToString());
                earn.serviceCommission = double.Parse(dt.Rows[i]["ServiceCommission"].ToString());
                earn.finalSalonEarning = double.Parse(dt.Rows[i]["FinalSalonEarning"].ToString());
                earn.bankName = dt.Rows[i]["BankName"].ToString();
                earn.accountName = dt.Rows[i]["AccountName"].ToString();
                earn.accountNumber = dt.Rows[i]["AccountNumber"].ToString();
                earn.earningPaymentStatus = dt.Rows[i]["EarningPaymentStatus"].ToString();
                earn.datePaid = dt.Rows[i]["DatePaid"].ToString();
                earn.numericalDatePaid = dt.Rows[i]["NumericalDatePaid"].ToString();

                // Add each earning to the list
                earnings.Add(earn);
            }

            da.Dispose();
            dt.Clear();
            conn.Close();

            // Return the earnings
            return earnings;
        }

        public static double GetTotalValueOfEarnings(string salonEmail, string status)
        {
            List<Earning> earnings = GetEarnings(salonEmail);
            double totalValueOfEarnings = 0;

            if (String.IsNullOrEmpty(status) || status.Equals("All"))
            {
                // Here, we will compute the total value of all earnings
                foreach (Earning earn in earnings)
                {
                    // Add each final earning to the total value
                    totalValueOfEarnings += earn.finalSalonEarning;
                }                
            }
            else
            {
                List<Earning> selectedEarnings = earnings.Where(r => r.earningPaymentStatus == status).ToList();
                // Here, we will compute the total value of all selected earnings
                foreach (Earning earn in selectedEarnings)
                {
                    // Add each final earning to the total value
                    totalValueOfEarnings += earn.finalSalonEarning;
                }    
            }

            return totalValueOfEarnings;
        }

        #endregion
    }
}