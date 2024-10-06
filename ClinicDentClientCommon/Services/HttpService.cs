using ClinicDentClientCommon.Exceptions;
using ClinicDentClientCommon.Model;
using ClinicDentClientCommon.RequestAnswers;
using ClinicDentClientCommon.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
namespace ClinicDentClientCommon.Services
{
    public static class HttpService
    {
        

        public static HttpClient httpClient;
        static MediaTypeFormatter[] bsonFormatting;
        static MediaTypeFormatter BsonFormatter;
        static MediaTypeWithQualityHeaderValue bsonHeaderValue;
        static MediaTypeWithQualityHeaderValue octetStreamHeaderValue;
        static HttpService()
        {

            BsonFormatter = new BsonMediaTypeFormatter();
            bsonFormatting = new MediaTypeFormatter[] { BsonFormatter };
            bsonHeaderValue = new MediaTypeWithQualityHeaderValue("application/bson");
            octetStreamHeaderValue = new MediaTypeWithQualityHeaderValue("application/octet-stream");
        }
        public static async Task<ScheduleRecordsForDayInCabinet> GetSchedule(DateTime date, string cabinetId)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            string dateStr = date.ToString(SharedData.DateTimePattern);



            HttpResponseMessage responseMessage = await httpClient.GetAsync($"Schedule/getRecordsForDay/{dateStr}/{cabinetId}").ConfigureAwait(false);
            if (responseMessage.IsSuccessStatusCode == false)
            {
                throw new Exception($"List<Schedule> GetSchedule(date={date.ToString()}; cabinetId={cabinetId}). Status code: {responseMessage.StatusCode}");
            }
            ScheduleRecordsForDayInCabinet receivedRecords = await responseMessage.Content.ReadAsAsync<ScheduleRecordsForDayInCabinet>(bsonFormatting).ConfigureAwait(false);
            return receivedRecords;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="patientIdToSet"></param>
        /// <param name="curePlanToSet"></param>
        /// <param name="lastModifiedDateTime"></param>
        /// <returns>new patient's lastmodified datetime from server</returns>
        /// <exception cref="ConflictException"></exception>
        /// <exception cref="Exception"></exception>
        public static async Task<string> PutPatientCurePlan(int patientIdToSet, string curePlanToSet, string lastModifiedDateTime)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            ChangeCurePlanRequest r = new ChangeCurePlanRequest()
            {
                CurePlan = curePlanToSet,
                PatientId = patientIdToSet,
                LastModifiedDateTime = lastModifiedDateTime
            };
            HttpResponseMessage result = await httpClient.PutAsync($"Patients/changeCurePlan", r, BsonFormatter).ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    throw new ConflictException((await result.Content.ReadAsAsync<ServerErrorMessage>(bsonFormatting).ConfigureAwait(false)).errorMessage);
                }
                if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new NotFoundException((await result.Content.ReadAsAsync<ServerErrorMessage>(bsonFormatting).ConfigureAwait(false)).errorMessage);
                }
                throw new Exception($"void PutPatientCurePlan(patientIdToSet={patientIdToSet},curePlanToSet={curePlanToSet}). Status code: {result.StatusCode}");
            }
            return await result.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        public static async Task<List<Stage>> GetPatientStages(int patientId)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Stages/{patientId}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"List<Stage> GetPatientStages(patientId = {patientId}). Status code: {result.StatusCode}");
            }
            List<Stage> stages = (await result.Content.ReadAsAsync<List<StageDTO>>(bsonFormatting).ConfigureAwait(false)).Select(d => new Stage(d)).ToList();
            return stages;
        }
        public static async Task<List<Stage>> GetManyStages(List<int> stagesId)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);

            string body = String.Join(",", stagesId);
            StringContent str = new StringContent(String.Join(",", stagesId), Encoding.UTF8, "text/plain");
            HttpResponseMessage result = await httpClient.PostAsync($"Stages/getMany", str).ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"List<Stage> GetManyStages(stagesId = {body}). Status code: {result.StatusCode}");
            }
            List<Stage> stages = (await result.Content.ReadAsAsync<List<StageDTO>>(bsonFormatting).ConfigureAwait(false)).Select(d => new Stage(d)).ToList();
            return stages;
        }
        public static async Task<Patient> GetPatient(int patientId)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Patients/{patientId}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                Patient receivedPatient = await result.Content.ReadAsAsync<Patient>(bsonFormatting).ConfigureAwait(false);
                return receivedPatient;
            }
            else
            {
                throw new Exception($"Patient GetPatient(patientId = {patientId}). Status code: {result.StatusCode}");
            }
        }
        public static async Task<PatientsToClient> GetPatients(string selectedStatus, string selectedSortDescription, int selectedPage, int patientsPerPage, string searchText)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Patients/{selectedStatus}/{selectedSortDescription}/{selectedPage}/{patientsPerPage}/{searchText}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsAsync<PatientsToClient>(bsonFormatting).ConfigureAwait(false);
            }
            else
            {
                throw new Exception($"PatientsToClient GetPatients( ... ). Status code: {result.StatusCode}");
            }
        }
        public static async Task<PatientsToClient> GetPatientsByImage(int imageId, string selectedStatus, string selectedSortDescription, int selectedPage, int patientsPerPage, string searchText)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Patients/byImageId/{imageId}/{selectedStatus}/{selectedSortDescription}/{selectedPage}/{patientsPerPage}/{searchText}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsAsync<PatientsToClient>(bsonFormatting).ConfigureAwait(false);
            }
            else
            {
                throw new Exception($"PatientsToClient GetPatientsByImage( ... ). Status code: {result.StatusCode}");
            }
        }
        public static async Task<PatientsToClient> GetPatients(string selectedStatus, string selectedSortDescription, int selectedPage, int patientsPerPage, string searchText, int doctorId)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Patients/{selectedStatus}/{selectedSortDescription}/{selectedPage}/{patientsPerPage}/{searchText}/{doctorId}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsAsync<PatientsToClient>(bsonFormatting).ConfigureAwait(false);
            }
            else
            {
                throw new Exception($"PatientsToClient GetPatients( ... doctorId={doctorId} ). Status code: {result.StatusCode}");
            }
        }
        public static async Task<DebtPatientsToClient> GetDebtors(string selectedSortDescription, int selectedPage, int patientsPerPage, string searchText, int doctorId)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Patients/debtors/{selectedSortDescription}/{selectedPage}/{patientsPerPage}/{searchText}/{doctorId}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsAsync<DebtPatientsToClient>(bsonFormatting).ConfigureAwait(false);
            }
            else
            {
                throw new Exception($"DebtPatientsToClient GetDebtors( ... doctorId={doctorId} ). Status code: {result.StatusCode}");
            }
        }
        public static async Task<string[]> GetTenantList()
        {
            HttpClient httpClient = CreateHttpClient(ConfigData.ServerAddress, TimeSpan.FromSeconds(10));
            HttpResponseMessage result = null;
            try
            {
                result = await httpClient.GetAsync($"Account/tenantNames").ConfigureAwait(false);
            }
            catch (Exception)
            {
                httpClient.Dispose();
                httpClient = CreateHttpClient(ConfigData.LanServerAddress, TimeSpan.FromSeconds(10));
                result = await httpClient.GetAsync($"Account/tenantNames").ConfigureAwait(false);
            }
            if (result.IsSuccessStatusCode == false)
            {
                httpClient.Dispose();
                throw new Exception("Can't retrieve tenant list");
            }
            httpClient.Dispose();
            return await result.Content.ReadAsAsync<string[]>(bsonFormatting).ConfigureAwait(false);
        }
        public static async Task<string> GetApiVersion()
        {
            HttpClient httpClient = CreateHttpClient(ConfigData.ServerAddress, TimeSpan.FromSeconds(10));
            HttpResponseMessage result = null;
            try
            {
                result = await httpClient.GetAsync($"Account/apiVersion").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                httpClient.Dispose();
                httpClient = CreateHttpClient(ConfigData.LanServerAddress, TimeSpan.FromSeconds(10));
                result = await httpClient.GetAsync($"Account/apiVersion").ConfigureAwait(false);
            }
            if (result.IsSuccessStatusCode == false)
            {
                httpClient.Dispose();
                throw new Exception("Can't retrieve server version");
            }
            httpClient.Dispose();
            return await result.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        public static async Task<Doctor> Authenticate(LoginModel loginModel)
        {

            httpClient = CreateHttpClient(ConfigData.ServerAddress, TimeSpan.FromSeconds(10));
            HttpResponseMessage result = null;
            try
            {
                result = await httpClient.PostAsync($"Account/login", loginModel, BsonFormatter).ConfigureAwait(false);
            }
            catch (Exception)
            {
                httpClient.Dispose();
                httpClient = CreateHttpClient(ConfigData.LanServerAddress, TimeSpan.FromSeconds(10));
                result = await httpClient.PostAsync($"Account/login", loginModel, BsonFormatter).ConfigureAwait(false);
            }
            if (result.IsSuccessStatusCode == false)
            {
                httpClient.Dispose();
                return null;
            }
            Doctor doctor = await result.Content.ReadAsAsync<Doctor>(bsonFormatting).ConfigureAwait(false);
            httpClient.Dispose();
            httpClient = CreateHttpClient(httpClient.BaseAddress.ToString());
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", doctor.EncodedJwt);
            return doctor;
        }
        public static async Task<Doctor> Register(RegisterModel registerModel)
        {
            httpClient = CreateHttpClient(ConfigData.ServerAddress, TimeSpan.FromSeconds(10));
            HttpResponseMessage result = null;
            try
            {
                result = await httpClient.PostAsync($"Account/register", registerModel, BsonFormatter).ConfigureAwait(false);
            }
            catch (Exception)
            {
                httpClient.Dispose();
                httpClient = CreateHttpClient(ConfigData.LanServerAddress, TimeSpan.FromSeconds(10));
                result = await httpClient.PostAsync($"Account/register", registerModel, BsonFormatter).ConfigureAwait(false);
            }
            if (result.IsSuccessStatusCode == false)
            {
                httpClient.Dispose();
                throw new Exception("Not authorized");
            }
            Doctor doctor = await result.Content.ReadAsAsync<Doctor>(bsonFormatting).ConfigureAwait(false);
            httpClient.Dispose();
            httpClient = CreateHttpClient(httpClient.BaseAddress.ToString());
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", doctor.EncodedJwt);
            return doctor;
        }
        public static HttpClient CreateHttpClient(string serverAddress, TimeSpan? timeout = null)
        {
            HttpClient newHttpClient = new HttpClient();
            newHttpClient.DefaultRequestHeaders.Accept.Clear();
            newHttpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            newHttpClient.BaseAddress = new Uri(serverAddress);
            if (timeout != null)
            {
                newHttpClient.Timeout = timeout.Value;
            }
            return newHttpClient;
        }
        public static async Task DeleteScheduleRecord(int id)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.DeleteAsync($"Schedule/{id}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"void DeleteScheduleRecord(id = {id}). Status code: {result.StatusCode}");
            }
        }
        public static async Task DeleteStage(int id)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.DeleteAsync($"Stages/{id}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"void DeleteStage(id = {id}). Status code: {result.StatusCode}");
            }
        }
        public static async Task RemoveImageFromStage(int imageId, int stageId)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.DeleteAsync($"Stages/removePhotoFromStage/{imageId}/{stageId}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"void RemoveImageFromStage(imageId = {imageId},stageId = {stageId}). Status code: {result.StatusCode}");
            }
        }
        public static async Task DeleteImage(int id)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.DeleteAsync($"Images/{id}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"void DeleteImage(id={id}). Status code: {result.StatusCode}");
            }

        }
        public static async Task DeletePatient(int id)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.DeleteAsync($"Patients/{id}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"void DeletePatients(id={id}). Status code: {result.StatusCode}");
            }

        }
        public static async Task PutScheduleRecord(Schedule schedule)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.PutAsync($"Schedule", schedule, BsonFormatter).ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"void PutScheduleRecord(Schedule schedule). Status code: {result.StatusCode}");
            }
        }
        public static async Task PutStage(Stage s)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            StageDTO stageDTO = new StageDTO(s);
            HttpResponseMessage result = await httpClient.PutAsync($"Stages", stageDTO, BsonFormatter).ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"void PutStage(Stage s). Status code: {result.StatusCode}");
            }
        }
        public static async Task PutStages(List<Stage> s)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            List<StageDTO> stageDTO = s.Select(l => new StageDTO(l)).ToList();
            HttpResponseMessage result = await httpClient.PutAsync($"Stages/putMany", stageDTO, BsonFormatter).ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"void PutStages(List<Stage> s). Status code: {result.StatusCode}");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="ConflictException">returns ids of conflict stages separated by comma.Example "1,2,3,4"</exception>
        /// <exception cref="Exception"></exception>
        public static async Task<PutStagesRequestAnswer> PutStages(List<StageDTO> s)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            PutStagesRequest putStagesRequest = new PutStagesRequest
            {
                stageDTO = s
            };
            HttpResponseMessage result = await httpClient.PutAsync($"Stages/putMany", putStagesRequest, BsonFormatter).ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    PutStagesRequestAnswer putStagesRequestAnswer = await result.Content.ReadAsAsync<PutStagesRequestAnswer>(bsonFormatting).ConfigureAwait(false);
                    throw new ConflictException("", putStagesRequestAnswer);
                }
                throw new Exception($"void PutStages(List<StageDTO> s). Status code: {result.StatusCode}");
            }
            else
            {
                PutStagesRequestAnswer putStagesRequestAnswer = await result.Content.ReadAsAsync<PutStagesRequestAnswer>(bsonFormatting).ConfigureAwait(false);
                return putStagesRequestAnswer;
            }
        }
        public static async Task<Schedule> PostScheduleRecord(Schedule newRecord)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.PostAsync($"Schedule", newRecord, BsonFormatter).ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"Schedule PostScheduleRecord(Schedule newRecord). Status code: {result.StatusCode}");
            }
            Schedule record = await result.Content.ReadAsAsync<Schedule>(bsonFormatting).ConfigureAwait(false);
            return record;
        }
        public static async Task<Stage> PostStage(Stage stage)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            StageDTO stageDTO = new StageDTO(stage);
            HttpResponseMessage result = await httpClient.PostAsync($"Stages", stageDTO, BsonFormatter).ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"Stage PostStage(Stage stage). Status code: {result.StatusCode}");
            }
            StageDTO stageFromServer = await result.Content.ReadAsAsync<StageDTO>(bsonFormatting).ConfigureAwait(false);
            return new Stage(stageFromServer);
        }
        public static async Task<Image> PostImage(Image image)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.PostAsync($"Images", image, BsonFormatter).ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"Image PostImage(Image image). Status code: {result.StatusCode}");
            }
            Image imageFromServer = await result.Content.ReadAsAsync<Image>(bsonFormatting).ConfigureAwait(false);
            return imageFromServer;
        }
        public static async Task<Patient> PostPatient(Patient patient)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.PostAsync($"Patients", patient, BsonFormatter).ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"Patient PostPatient(Patient patient). Status code: {result.StatusCode}");
            }
            patient = await result.Content.ReadAsAsync<Patient>(bsonFormatting).ConfigureAwait(false);
            return patient;
        }
        public static async Task<string> PutPatient(Patient patient)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.PutAsync($"Patients", patient, BsonFormatter).ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    throw new ConflictException((await result.Content.ReadAsAsync<ServerErrorMessage>(bsonFormatting).ConfigureAwait(false)).errorMessage);
                }
                throw new Exception($"Patient PutPatient(Patient patient). Status code: {result.StatusCode}");
            }
            return await result.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        public static async Task AddImageToStage(int imageId, int stageId)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Stages/addPhotoToStage/{imageId}/{stageId}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"void AddImageToStage(imageId = {imageId},stageId = {stageId}). Status code: {result.StatusCode}");
            }
        }
        public static async Task<List<StageAsset>> GetStageAssets()
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Stages/stageAssets").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"List<StageAsset> GetStageAssets(). Status code: {result.StatusCode}");
            }
            List<StageAsset> receivedAssets = await result.Content.ReadAsAsync<List<StageAsset>>(bsonFormatting).ConfigureAwait(false);
            return receivedAssets;
        }
        public static async Task PostStageAsset(StageAsset stageAsset)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            stageAsset.Id = 0;
            if (stageAsset.Value == null)
            {
                stageAsset.Value = "";
            }
            HttpResponseMessage result = await httpClient.PostAsync($"Stages/stageAsset", stageAsset, BsonFormatter).ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"void PostStageAsset(StageAsset stageAsset). Status code: {result.StatusCode}");
            }
            int primaryKey = Convert.ToInt32(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            stageAsset.Id = primaryKey;
        }
        public static async Task<List<Stage>> GetRelatedStagesToSchedule(int scheduleId)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Schedule/getRelatedStages/{scheduleId}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"List<Stage> GetRelatedStagesToSchedule(scheduleId={scheduleId}). Status code: {result.StatusCode}");
            }
            List<Stage> receivedStages = (await result.Content.ReadAsAsync<List<StageDTO>>(bsonFormatting).ConfigureAwait(false)).Select(d => new Stage(d)).ToList();
            receivedStages.Reverse();
            return receivedStages;
        }
        public static async Task<byte[]> GetImageOriginalBytes(int imageId)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(octetStreamHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Images/getOriginalBytes/{imageId}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"byte[] GetImageOriginalBytes(imageId={imageId}). Status code: {result.StatusCode}");
            }
            return await result.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }
        public static async Task<Cabinet[]> GetCabinets()
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Schedule/getCabinets").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"Cabinet[] GetCabinets(). Status code: {result.StatusCode}");
            }
            Cabinet[] receivedCabinets = (await result.Content.ReadAsAsync<Cabinet[]>(bsonFormatting).ConfigureAwait(false)).ToArray();
            return receivedCabinets;
        }
        public static async Task<Image[]> GetImagesForStage(int stageId)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Stages/getPhotosForStage/{stageId}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"Image[] GetImagesForStage(stageId={stageId}). Status code: {result.StatusCode}");
            }
            return await result.Content.ReadAsAsync<Image[]>(bsonFormatting).ConfigureAwait(false);
        }
        public static async Task<Doctor[]> GetDoctors()
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Doctors/getAll").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"Doctor[] GetDoctors(). Status code: {result.StatusCode}");
            }
            return await result.Content.ReadAsAsync<Doctor[]>(bsonFormatting).ConfigureAwait(false);
        }
        public static async Task<List<Schedule>> GetPatientFutureAppointments(int patientId)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Schedule/getPatientFutureAppointments/{patientId}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($" List<Schedule> GetPatientFutureAppointments(int patientId = {patientId}). Status code: {result.StatusCode}");
            }
            return await result.Content.ReadAsAsync<List<Schedule>>(bsonFormatting).ConfigureAwait(false);
        }
        public static async Task<ImagesToClient> GetImages(int selectedPage, int photosPerPage, int doctorId, ImageType imageType)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Images/{selectedPage}/{photosPerPage}/{doctorId}/{imageType}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"ImagesToClient GetImages(selectedPage={selectedPage}, photosPerPage={photosPerPage}, doctorId={doctorId}, imageType={imageType}). Status code: {result.StatusCode}");
            }
            return await result.Content.ReadAsAsync<ImagesToClient>(bsonFormatting).ConfigureAwait(false);
        }
        public static async Task RenameImage(int imageId, string newName)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            StringContent str = new StringContent(newName, Encoding.UTF8, "text/plain");
            HttpResponseMessage result = await httpClient.PostAsync($"Images/changeImageName/{imageId}", str).ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"void RenameImage(imageId={imageId}, newName={newName}). Status code: {result.StatusCode}");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stageId"></param>
        /// <param name="mark">1 means true, 0 means false</param>
        /// <exception cref="Exception"></exception>
        public static async Task StageMarkSentViaMessager(int stageId, int mark)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Stages/sentViaMessager/{stageId}/{mark}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"void StageMarkSentViaMessager(int stageId={stageId}, int mark={mark}). Status code: {result.StatusCode}");
            }
        }
        public static async Task<WeekMoneySummaryRequestAnswer> GetWeekMoneySummary(DateTime sunday)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            WeekMoneySummaryRequest r = new WeekMoneySummaryRequest()
            {
                AnySunday = sunday
            };
            HttpResponseMessage result = await httpClient.PutAsync($"Schedule/weekMoneySummary", r, BsonFormatter).ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"WeekMoneySummaryRequestAnswer GetWeekMoneySummary(sunday={sunday}). Status code: {result.StatusCode}");
            }
            return await result.Content.ReadAsAsync<WeekMoneySummaryRequestAnswer>(bsonFormatting).ConfigureAwait(false);
        }

        public static async Task<int> GetFutureWorkingMinutes(int cabinetId)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            HttpResponseMessage result = await httpClient.GetAsync($"Statistics/futureWorkingMinutes/{cabinetId}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"int GetFutureWorkingHours(int cabinetId = {cabinetId}). Status code: {result.StatusCode}");
            }
            return Convert.ToInt32(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        public static async Task<int> GetFutureUniquePatients(int cabinetId)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            HttpResponseMessage result = await httpClient.GetAsync($"Statistics/futureUniquePatientsAmount/{cabinetId}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"int  GetFutureUniquePatients(int cabinetId = {cabinetId}). Status code: {result.StatusCode}");
            }
            return Convert.ToInt32(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
        }
        public static async Task<ToothUnderObservation> GetToothUnderObservation(int toothObservationId)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync($"Observations/tooth/{toothObservationId}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"ToothUnderObservation GetToothUnderObservation(toothObservationId = {toothObservationId}). Status code: {result.StatusCode}");
            }
            return await result.Content.ReadAsAsync<ToothUnderObservation>(bsonFormatting).ConfigureAwait(false);
        }
        public static async Task<List<ToothUnderObservation>> GetAllToothUnderObservation()
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.GetAsync("Observations/allTooth").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"List<ToothUnderObservation> GetAllToothUnderObservation(). Status code: {result.StatusCode}");
            }
            return await result.Content.ReadAsAsync<List<ToothUnderObservation>>(bsonFormatting).ConfigureAwait(false);
        }
        public static async Task<int> PostToothUnderObservation(ToothUnderObservation newRecord)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            HttpResponseMessage result = await httpClient.PostAsync($"Observations/tooth", newRecord, BsonFormatter).ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"int PostToothUnderObservation(ToothUnderObservation newRecord). Status code: {result.StatusCode}");
            }
            return Convert.ToInt32(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
        }
        public static async Task PutToothUnderObservation(ToothUnderObservation updatedRecord)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.PutAsync($"Observations/tooth", updatedRecord, BsonFormatter).ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"void PutToothUnderObservation(ToothUnderObservation updatedRecord). Status code: {result.StatusCode}");
            }
        }
        public static async Task DeleteToothUnderObservation(int toothObservationId)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(bsonHeaderValue);
            HttpResponseMessage result = await httpClient.DeleteAsync($"Observations/tooth/{toothObservationId}").ConfigureAwait(false);
            if (result.IsSuccessStatusCode == false)
            {
                throw new Exception($"ToothUnderObservation DeleteToothUnderObservation(toothObservationId = {toothObservationId}). Status code: {result.StatusCode}");
            }
        }
    }
}
