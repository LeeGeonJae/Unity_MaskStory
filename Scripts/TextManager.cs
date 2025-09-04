using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Audio;

[System.Serializable]
public class StoryLine
{
    public string ID;
    public string Content;
}

public class TextManager : MonoBehaviour
{
    // 텍스트 쓰기
    [Header("TextWrite")]
    public TextMeshProUGUI textUI;
    public float charDelay = 0.05f;   // 글자 간격 시간
    public float fadeTime = 0.2f;     // 페이드 시간

    // 엑셀 로드
    [Header("Excel Loader")]
    public TextAsset csvFile;
    public List<StoryLine> storyList = new List<StoryLine>();

    [Header("Sound")]
    AudioSource audioSource;
    [SerializeField] AudioClip typingAudioClip;

    // 텍스트 저장소
    Dictionary<string, string> storyBoard = new Dictionary<string, string>();

    // 텍스트 옵션
    Coroutine typingCoroutine;
    bool skip = false;
    Reword newReword;
    Penalty newPenalty;
    string addText;

    void Awake()
    {
        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(',');
            if (row.Length < 2)
            {
                continue;
            }

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

    private void Start()
    {
        GameManager.instance.storyManager.OnReword.AddListener(GetReword);
        GameManager.instance.storyManager.OnPenalty.AddListener(GetPenalty);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.resource = typingAudioClip;
        audioSource.loop = false;

    }

    private void GetReword(Reword reword)
    {
        if (reword.RewordValue != 0)
        {
            newReword = reword;
        }
    }
    private void GetPenalty(Penalty penalty)
    {
        if (penalty.PenaltydValue != 0)
        {
            newPenalty = penalty;
        }
    }

    public void ShowText(string fullText, GameStateType nextGameStateType)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        addText = "";
        if (newReword != null)
        {
            switch (newReword.StatType)
            {
                case PlayerStatType.Hp:
                    addText = "체력+" + newReword.RewordValue + "\n\n";
                    break;
                case PlayerStatType.Power:
                    addText = "힘+" + newReword.RewordValue + "\n\n";
                    break;
                case PlayerStatType.Intelligence:
                    addText = "지능+" + newReword.RewordValue + "\n\n";
                    break;
                case PlayerStatType.Agility:
                    addText = "민첩+" + newReword.RewordValue + "\n\n";
                    break;
            }
            typingCoroutine = StartCoroutine(TypeText(addText + storyBoard[fullText], nextGameStateType));
            newReword = null;
        }
        else if (newPenalty != null)
        {
            switch (newPenalty.StatType)
            {
                case PlayerStatType.Hp:
                    addText = "체력-" + newPenalty.PenaltydValue + "\n\n";
                    break;
                case PlayerStatType.Power:
                    addText = "힘-" + newPenalty.PenaltydValue + "\n\n";
                    break;
                case PlayerStatType.Intelligence:
                    addText = "지능-" + newPenalty.PenaltydValue + "\n\n";
                    break;
                case PlayerStatType.Agility:
                    addText = "민첩-" + newPenalty.PenaltydValue + "\n\n";
                    break;
            }
            typingCoroutine = StartCoroutine(TypeText(addText + storyBoard[fullText], nextGameStateType));
            newPenalty = null;
        }
        else
        {
            typingCoroutine = StartCoroutine(TypeText(storyBoard[fullText], nextGameStateType));
        }

        addText = "";
    }

    IEnumerator TypeText(string fullText, GameStateType nextGameStateType)
    {
        textUI.text = "";
        for (int i = 0; i < fullText.Length; i++)
        {
            if (skip)
            {
                textUI.text = fullText.Replace("\\n", "\n"); // 스킵 시에도 줄바꿈 적용
                break;
            }

            textUI.text += fullText[i] == '\\' && i + 1 < fullText.Length && fullText[i + 1] == 'n' ? "\n" : fullText[i].ToString();
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
            {
                vertexColors[vertexIndex + j] = new Color32(255, 255, 255, 0);
            }

            textUI.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            float t = 0;
            while (t < fadeTime && !skip)
            {
                t += Time.deltaTime;
                byte alpha = (byte)Mathf.Lerp(0, 255, t / fadeTime);
                for (int j = 0; j < 4; j++)
                {
                    vertexColors[vertexIndex + j].a = alpha;
                }

                textUI.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                yield return null;
            }

            audioSource.Play();
            yield return new WaitForSeconds(charDelay);
        }

        skip = false;
        GameManager.instance.SetGameState(nextGameStateType);
    }

    public void SkipText()
    {
        skip = true;
    }
}
