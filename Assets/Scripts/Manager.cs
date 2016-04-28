using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Manager : MonoBehaviour
{
    // -- CONSTANTS --
    public List<string> teams =
             new List<string> {
        "Blue",
        "Red"
    };

    public Dictionary<string, Color> teamColors =
       new Dictionary<string, Color> {
        {"Blue", new Color(58f/255f, 112f/255f, 225f/255f, 1f)},
        {"Red", Color.red}
    };

    public Dictionary<string, int> teamLayers =
       new Dictionary<string, int> {
       {"Blue", 9},
       {"Red", 10}
    };

    public Dictionary<string, List<string>> teamTextures =
       new Dictionary<string, List<string>> {
        {"Blue", new List<string>{"blue_bee", "blue_bee2"}},
        {"Red",  new List<string>{"red_bee",  "red_bee2"}}
    };

    public float manaTime = 15f;
    const int countdownLength = 3;
    const int pointLimit = 5;

    // -- VARIABLES --
    public Dictionary<string, Vector2> spawnLocations =
       new Dictionary<string, Vector2> {
       {"Blue", new Vector2(0, 4f)},
       {"Red", new Vector2(0, -4f)}
    };
	public Dictionary<string, int> teamScores;
	public Dictionary<string, int> teamManas;
	public Dictionary<string, GUIText> teamScoreText;
	public Dictionary<string, ManaBar> teamManaBars;
    public Dictionary<string, GameObject> teamBases;
    public List<Player> allPlayers =
       new List<Player>();
    public Flag flag;
    public GUIText tutorialGUIText;
    public GUIText tutorialSkipGUIText;
    public GUIText countdownGUIText;
    public GUITexture countdownBackground;
    public Texture pauseImage;

    private float timePassed=0;
    public bool roundStarted = false;
    public bool itemPickups = false;
    private bool isPause = false;
    private bool tutorial = true;
    private Sound tutorialSong;

    static public Manager S {
        get {
            return GameObject.FindObjectOfType(typeof(Manager)) as Manager;
        }
    }

    // -- SETUP --

    void Start() {
        Time.timeScale = 1.2f;
        teamScores = InitScores(teams);
        teamScoreText = InitScoreText(teams);
        SpawnPlayers();
        CreateCountdown();

        teamManaBars = InitManaBars (teams);

        teamBases = teams.ToDictionary(team => team,                                 //key
                                       team => GameObject.FindWithTag(team+"Side")); //value
        flag = GameObject.FindObjectOfType(typeof(Flag)) as Flag;
           
        StartCoroutine(TutorialCoroutine());
    }

    Dictionary<string, int> InitScores(List<string> teamList) {
        return teamList.ToDictionary(t => t, t => 0);
    }
	Dictionary<string, int> InitManas(List<string> teamList) {
		return teamList.ToDictionary(t => t, t => 0);
	}
	
	Dictionary<string, GUIText> InitScoreText(List<string> teamList) {
        GameObject scoreBoard = GameObject.FindWithTag("ScoreBoard");
        return teamList.ToDictionary(t => t, t => {
            GameObject go = Instantiate(Resources.Load("ScoreText")) as GameObject;
            GUIText gt = go.GetComponent<GUIText>();
            float width = .8f / teamList.Count();
            float index = (float)teamList.IndexOf(t);
            go.transform.parent = scoreBoard.transform;
            go.transform.localScale = new Vector2(width, 1);
            go.transform.localPosition = new Vector3(index*width - 0.2f, 0.1f, 1);
            gt.color = teamColors[t];
            gt.fontSize = (int)(Screen.height * 0.04);
            return gt;
        });
    }

	Dictionary<string, ManaBar> InitManaBars(List<string> teamList) {
		return teamList.ToDictionary(t => t, t => {
			GameObject go = Instantiate(Resources.Load("ManaBar")) as GameObject;
			ManaBar mb = go.GetComponent<ManaBar>();
			mb.team = t;
			GUIText gt = go.GetComponentInChildren<GUIText>();
            if (itemPickups) {
                Destroy(gt.gameObject);
            }
			float index = (float)teamList.IndexOf(t);
			go.transform.position = new Vector3(Mathf.Abs(index - 0.04f), 0.5f, -1);
			gt.color = teamColors[t];
            gt.fontSize = Screen.width / 100;
			return mb;
		});
	}
	
	void SpawnPlayers() {
        if (PlayerOptions.teamForPlayer.Count == 0) {
            //for testing without needing to load menu
            PlayerOptions.teamForPlayer[0] = "Red";
            PlayerOptions.teamForPlayer[1] = "Blue";
            PlayerOptions.teamForPlayer[2] = "Red";
            PlayerOptions.teamForPlayer[3] = "Blue";
        }
		PlayerOptions.teamForPlayer.ToList().ForEach(kvp => {
            allPlayers.Add(SpawnPlayer(kvp.Value, kvp.Key));
        });
    }

    Player SpawnPlayer(string team, int index) {
        GameObject playerGO = Instantiate(Resources.Load("Player")) as GameObject;
        Player player = playerGO.GetComponent<Player>();

        Vector3 initialPos = spawnLocations[team];
        spawnLocations[team] = Quaternion.Euler(0,0,30) * spawnLocations[team];
        playerGO.transform.position = player.initialPos = initialPos;
        player.heading = player.initialHeading = playerGO.transform.position * -1;
        playerGO.layer = teamLayers[team];
        player.team = team;
        player.number = index;

        // set sprite:
        int idxInTeam = allPlayers.Where(p => p.team == team).Count();
        Sprite sp = Resources.Load(teamTextures[team][idxInTeam], typeof(Sprite)) as Sprite;
        player.spriteRenderer.sprite = sp;

        return player;
    }

    void CreateCountdown() {
        GameObject countdownParent = Instantiate(Resources.Load("Countdown")) as GameObject;
        countdownGUIText = countdownParent.GetComponentInChildren<GUIText>();
        countdownBackground = countdownParent.GetComponentInChildren<GUITexture>();
        countdownGUIText.fontSize = Screen.width / 18;
    }

    // -- UPDATE --

    public void Update() {
        // blinking tutorial text
        if(tutorialGUIText.enabled) Blink();

        if(!tutorial && (Input.GetButtonDown("start") || Input.GetKeyDown(KeyCode.Return))) {
            isPause = !isPause;
            if(isPause)
              Time.timeScale = 0;
            else
              Time.timeScale = 1;
        }

        teamScoreText.ToList().ForEach(kvp => {
            GUIText gt = kvp.Value;
            int score = teamScores[kvp.Key];
            gt.text = score.ToString();
        });
        timePassed += Time.deltaTime;

        if (!itemPickups) {
            if (timePassed >= manaTime) {
                int lol =0;
                teams.ForEach (team => {
                    if(teamManas[team]==3)
                        lol+=1;
                    teamManas [team] += 1;
                    teamManas [team] = Mathf.Min (teamManas [team], 3);
                });
                if(lol!=2)
                    AudioManager.Main.PlayNewSound("manaupall");
                timePassed = 0;
            }
            teamManaBars.ToList().ForEach (kvp => {
                ManaBar gt = kvp.Value;
                gt.currMana = teamManas [kvp.Key];
            });
        }
    }

    private void OnGUI() {
        Rect fullScreen = new Rect(0, 0, Screen.width, Screen.height);

        if(isPause) {
            GUI.Window(0, fullScreen, PauseMenu, pauseImage);
        }
    }

    private void PauseMenu(int windowID) {
        
    }

	// -- GAME EVENTS --
    public void DidScore(Player scorer) {
        if (!tutorial)
            StartCoroutine(ScoreCoroutine(scorer.team));
    }

    private IEnumerator ScoreCoroutine(string team) {
        allPlayers.ForEach(p => p.Die());
        flag.GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(1);        
        if (++teamScores[team] >= pointLimit) {
            countdownGUIText.enabled = countdownBackground.enabled = true;
            countdownGUIText.text = team + " team wins!";
            yield return new WaitForSeconds(2);
            Application.LoadLevel(0);
        } else {
            StartNewRound(false, team);
        }
    }

    public void StartGame() {
        allPlayers.ForEach(p => p.Die());
        tutorialSong.playing = false;
        tutorial = false;
        tutorialGUIText.enabled = false;
        tutorialSkipGUIText.enabled = false;
        AudioManager.Main.PlayNewSound("Background", loop: true);
        StartNewRound(true, null);
    }

    public void StartNewRound(bool showRules, string teamScored)
    {
        roundStarted = false;
        allPlayers.ForEach(p => {
            p.Reset();
            p.frozen = true;
        });
        flag.Reset();
        (GameObject.FindObjectsOfType(typeof(Turret)) as Turret[])
           .ToList()
           .ForEach(t => t.Deactivate());
        (GameObject.FindGameObjectsWithTag("ItemPickup") as GameObject[])
                   .ToList()
                   .ForEach(Destroy);
        timePassed = 0;
		foreach (GameObject spawnpt in GameObject.FindGameObjectsWithTag("ItemSpawn")) {
			ItemSpawn its=spawnpt.GetComponent<ItemSpawn>();
			its.spawned=false;
		}

        StartCoroutine(CountdownCoroutine(showRules, teamScored));
    }

    public IEnumerator CountdownCoroutine(bool showRules, string teamScored)
    {
        countdownGUIText.enabled = countdownBackground.enabled = true;
        teamManas = InitManas(teams);

        if (showRules) {
            countdownGUIText.text = "Bring the pollen\nto your flower!";
            yield return new WaitForSeconds(2);

            countdownGUIText.text = "First to " + pointLimit + " points wins!";
            yield return new WaitForSeconds(1.5f);
        }
        if (teamScored != null) {
            countdownGUIText.text = teamScored + " team scored!";
            yield return new WaitForSeconds(1);
        }

        for(int i=countdownLength; i > 0; i--) {
            countdownGUIText.text = i.ToString();
            yield return new WaitForSeconds(0.7f);
        }
        countdownGUIText.text = "Start!\n\n\n";

        allPlayers.ForEach(p => p.frozen = false);
        roundStarted = true;
        countdownBackground.enabled = false;

        yield return new WaitForSeconds(1);
        countdownGUIText.enabled = false;
        countdownGUIText.text = null;
    }

    public IEnumerator TutorialCoroutine() {
        countdownBackground.enabled = countdownGUIText.enabled = false;
        teamManas = InitManas(teams);
        tutorialSong = AudioManager.Main.PlayNewSound("Tutorial", loop: true);

        GameObject tutorialParent = Instantiate(Resources.Load("Tutorial")) as GameObject;
        GUIText[] tutorialGUITexts = tutorialParent.GetComponentsInChildren<GUIText>();

        tutorialGUIText = tutorialGUITexts[0];
        tutorialSkipGUIText = tutorialGUITexts[1];
        
        tutorialGUIText.fontSize = Screen.width / 30;
        tutorialSkipGUIText.fontSize = Screen.width / 34;

        tutorialSkipGUIText.text = "Press Start to skip tutorial";

        List<KeyValuePair<string, float>> messagesAndWaitTimes =
        new List<KeyValuePair<string, float>> {
            new KeyValuePair<string, float> ("Move around with the left stick", 4f),
            new KeyValuePair<string, float> ("Tackle your enemies with the right stick", 6f),
            new KeyValuePair<string, float> ("(While tackling, you're invincible to other tackles)", 5f),
            new KeyValuePair<string, float> ("Collect purple mana powerups to use special attacks!", 5f),
            new KeyValuePair<string, float> ("Press Y for a speed boost!", 5f),
            new KeyValuePair<string, float> ("Press B to create a drone to fight for you", 5f),
            new KeyValuePair<string, float> ("Press A to create a turret", 5f),
            new KeyValuePair<string, float> ("Press X to knock back turrets and enemies", 5f),
            new KeyValuePair<string, float> ("Press Start when you're ready to play!", 5f),
            new KeyValuePair<string, float> ("", 30f)
        };

        foreach (KeyValuePair<string, float> p in messagesAndWaitTimes) {
            foreach (string k in teamManas.Keys.ToList()) {
                teamManas[k] = Mathf.Min(teamManas[k] + 1, 3);
            }
            tutorialGUIText.text = p.Key;
            for (int i=0; i<p.Value*20; ++i) {
                if (Input.GetButton("start") || Input.GetKey(KeyCode.Return)) {
                    StartGame();
                    yield break;
                }
                yield return new WaitForSeconds(0.045f);
            }
        }
    }

    private void Blink() {
        Color color = tutorialGUIText.material.color;
        color.a = Mathf.Round(Mathf.PingPong(Time.time * 1f, 1.0f));
        tutorialSkipGUIText.material.color = color;
    }   
}
