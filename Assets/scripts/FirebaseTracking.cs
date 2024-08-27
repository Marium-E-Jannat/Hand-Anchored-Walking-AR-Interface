using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseTracking: MonoBehaviour{
    private DatabaseReference databaseReference;

    public FirebaseTracking(List<string> datafields, List<Action<float>> callbacks){
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            for(int i = 0; i < datafields.Count; i++){
                ListenForChange(datafields[i], callbacks[i]);
            }
        });

        // // initialize values
        // for(int i = 0; i < datafields.Count; i++){
        //     Action <float> callback = callbacks[i];
        //     float initialVal = (float)databaseReference.Child(datafields[i]).GetValueAsync().Result.Value;
        //     callback(initialVal);
        // }
    }

    public void ListenForChange(string dataField, Action<float> callback){
        databaseReference.Child(dataField).ValueChanged += (object sender, ValueChangedEventArgs args) => { 
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            if (args.Snapshot.Exists)
            {
                Debug.Log(dataField + " from firebase is " + args.Snapshot.Value.ToString());
                if(float.TryParse(args.Snapshot.Value.ToString(), out float result))
                {
                    callback(result);
                }
            }
        };
    }
}
