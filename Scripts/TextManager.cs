using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class StoryLine
{
    public string ID;
    public string Content;
}

public class TextManager : MonoBehaviour
{
    [Header("TextWrite")]
    public TextMeshProUGUI textUI;
    public float charDelay = 0.05f;   // 글자 간격 시간
    public float fadeTime = 0.2f;     // 페이드 시간

    private Coroutine typingCoroutine;
    private bool skip = false;

    [Header("Excel Loader")]
    public TextAsset csvFile;
    public List<StoryLine> storyList = new List<StoryLine>();

    Dictionary<string, string> storyBoard = new Dictionary<string, string>();

    void Awake()
    {
        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(',');
            if (row.Length < 2) continue;
            storyList.Add(new StoryLine
            {
                ID = row[0],
                Content = row[1].Trim('"').Replace("\\n", "\n")
            });

            if (!storyBoard.ContainsKey(row[0]))
            {
                storyBoard.Add(row[0], row[1].Trim('"').Replace("\\n", "\n"));
            }
        }
    }

    public void ShowText(string fullText, bool compelete)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(storyBoard[fullText], compelete));
    }

    IEnumerator TypeText(string fullText, bool compelete)
    {
        textUI.text = "";
        for (int i = 0; i < fullText.Length; i++)
        {
            if (skip)
            {
                textUI.text = fullText.Replace("\\n", "\n"); // 스킵 시에도 줄바꿈 적용
                break;
            }

            textUI.text += fullText[i] == '\\' && i + 1 < fullText.Length && fullText[i + 1] == 'n'
                ? "\n" // "\n" 문자열을 실제 개행으로 변환
                : fullText[i].ToString();

            textUI.ForceMeshUpdate();

            int charIndex = textUI.text.Length - 1;
            TMP_CharacterInfo charInfo = textUI.textInfo.characterInfo[charIndex];

            // 개행 문자 같은 보이지 않는 문자는 스킵
            if (!charInfo.isVisible)
            {
                yield return null;
                continue;
            }

            var meshInfo = textUI.textInfo.meshInfo[charInfo.materialReferenceIndex];
            int vertexIndex = charInfo.vertexIndex;
            Color32[] vertexColors = meshInfo.colors32;

            for (int j = 0; j < 4; j++)
                vertexColors[vertexIndex + j] = new Color32(255, 255, 255, 0);

            textUI.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            float t = 0;
            while (t < fadeTime && !skip)
            {
                t += Time.deltaTime;
                byte alpha = (byte)Mathf.Lerp(0, 255, t / fadeTime);
                for (int j = 0; j < 4; j++)
                    vertexColors[vertexIndex + j].a = alpha;

                textUI.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                yield return null;
            }

            yield return new WaitForSeconds(charDelay);
        }

        skip = false;

        // 텍스트 완료
        if (compelete)
        {
            if (GameManager.instance.currentGameState != GameStateType.MoveNextStep)
            {
                GameManager.instance.SetGameState(GameStateType.StoryEvent_CompleteEvent);
            }
        }
        else
        {
            if (GameManager.instance.currentGameState != GameStateType.MoveNextStep)
            {
                GameManager.instance.SetGameState(GameStateType.StoryEvent_CompleteText);
            }
        }
    }

    public void SkipText()
    {
        skip = true;
    }
}
