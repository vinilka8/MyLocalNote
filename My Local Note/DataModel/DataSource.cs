using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;

namespace My_Local_Note.DataModel
{
    public class MapNote
    {
        public string Title { get; set; }
        public string Note { get; set; }
        //public DataTime Create { get; set; }
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
    }

    public class DataSource
    {
        private ObservableCollection<MapNote> mapNotes;
        const string fileName = "myMapNotes.json";
        public DataSource()
        {
            mapNotes = new ObservableCollection<MapNote>();

        }

        public async Task<ObservableCollection<MapNote>> GetMapNotes()
        {
            await ensureDataLoaded();
            return mapNotes;
        }

        private async Task ensureDataLoaded()
        {
            if (mapNotes.Count == 0)
                await getMapNoteDataAsync();

            return;
        }

        private async Task getMapNoteDataAsync()
        {
            if (mapNotes.Count != 0)
                return;

            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<MapNote>));

            try
            {
                using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(fileName))
                {
                    mapNotes = (ObservableCollection<MapNote>)jsonSerializer.ReadObject(stream);
                }
            }
            catch
            {
                mapNotes = new ObservableCollection<MapNote>();
            }
        }

        public async void AddMapNote(MapNote mapNote)
        {
            mapNotes.Add(mapNote);
            await saveMapNoteDataAsync();
        }

        public async void DeleteMapNote(MapNote mapNote)
        {
            mapNotes.Remove(mapNote);
            await saveMapNoteDataAsync();
        }

        private async Task saveMapNoteDataAsync()
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<MapNote>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(fileName,
                CreationCollisionOption.ReplaceExisting))
            {
                jsonSerializer.WriteObject(stream, mapNotes);
            }
        }

    }
}
