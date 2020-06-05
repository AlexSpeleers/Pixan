using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Animator crossFadeAnimator;
    [SerializeField] [Range(0.4f, 1f)] private float transitionTime;
    [Inject] readonly ZenjectSceneLoader sceneLoader;

    public void LoadScene(int sceneIndex) 
    {
        StartCoroutine(LoadLevelRoutine(sceneIndex));
    }

    IEnumerator LoadLevelRoutine(int sceneIndex) 
    {
        crossFadeAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneIndex);
    }
    public IEnumerator LoadScene(int sceneIndex, Player player)
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(LoadSceneRoutine(sceneIndex, player));
    }
    IEnumerator LoadSceneRoutine(int sceneIndex, Player player)
    {
        crossFadeAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        sceneLoader.LoadScene(sceneIndex, LoadSceneMode.Single,
            (container) => { container.BindInstance(player).WhenInjectedInto<Spawner>(); });
    }
}