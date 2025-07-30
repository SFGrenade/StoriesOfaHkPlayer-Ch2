using SFCore.Utils;
using UnityEngine;

namespace StoriesOfaHkPlayer_Ch2.MonoBehaviours;

public class StartGameReceiver : MonoBehaviour
{
    public void CustomStartMain()
    {
        GlobalCustomStartMain();
    }

    public static void GlobalCustomStartMain()
    {
        UIManager self = UIManager.instance;
        self.GetAttr<UIManager, GameManager>("gm").profileID = -2;
        self.StartNewGame(false, false);
    }
}