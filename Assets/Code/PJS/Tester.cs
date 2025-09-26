using UnityEngine;
using UnityEngine.SceneManagement;

public class Tester : MonoBehaviour
{
    [ContextMenu("saveTest")]
    public void Test() //TestComplete, removeThis
    {
        DataContructor.Instance.AddData<DataStructs>(new DataStructs()
        {
            _name = "asdf",
            Description = "¤·¤©³ª¤Ó¤©",
            floater = 0.2f,
            integer = 5,
        });

        Debug.Log(DataContructor.Instance.GetData<DataStructs>("asdf"));

        DataContructor.Instance.HandleSave(SceneManager.GetActiveScene());
    }
}
