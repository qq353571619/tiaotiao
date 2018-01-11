using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TiaoTiao : MonoBehaviour {

    private Rigidbody rig;
	private float power = 0;
	private bool isFly = false;
	private int score = 0;
	private GameObject preGameObject;
    private float len = 0;
	private Vector3 targetPos;
	private bool isGameOver = false;

	public Transform Camer;
    public GameObject Floor;
	public ParticleSystem Bomb;
	public Text Ttext;
	// Use this for initialization

	void Start () {
		rig = this.GetComponent<Rigidbody> ();
		targetPos = Camer.position;
	}
	
	// Update is called once per frame
	void Update () {

		CheckGameOver ();

		if (Input.GetMouseButton (0) && isFly==false) {
			
			float tiaoScale = transform.localScale.y - Time.deltaTime;
			if (tiaoScale < 0.3f) {
				tiaoScale = 0.3f;
			} else {
				power += 1000 * Time.deltaTime;
			}

			this.transform.localScale = new Vector3 (1,tiaoScale , 1);
		}

		if (Input.GetMouseButtonUp (0) && isFly == false) {
			rig.AddForce (new Vector3 (0, 0.5f, 0.5f) * power);
			transform.localScale = Vector3.one;
			isFly = true;
			rig.useGravity = true;
        }

		CamerFollow ();
	}

	void OnCollisionEnter(Collision col){
		
		if (col.gameObject == preGameObject) {
			return;
		}

		preGameObject = col.gameObject;
		Vector3 point = col.contacts[0].point;

        float outLine_low = preGameObject.transform.position.z - 1;
		if (point.y <= 0.4f || outLine_low >= point.z - 0.2f) {
            //游戏结束
			rig.Sleep();
			//模拟反向力，推动向后下走
			rig.AddForce (0, -100f, -100f);
			isGameOver = true;

		}else{
			//站起来
			transform.position = new Vector3(point.x,1.5f,point.z);
			transform.rotation = Quaternion.identity;
			rig.Sleep ();
			rig.useGravity = false;
			isFly = false;
            power = 0;
			targetPos.z += len;

			ShowScore ();
			PlayBomb ();
			CreateFloot();
        }
	}

    void CreateFloot() {

        len = Random.Range(4.0f, 6.0f);
        Vector3 newPos = new Vector3(0, 0, preGameObject.transform.position.z + len);
        Instantiate(Floor, newPos, Quaternion.identity);
       
    }

	void PlayBomb(){
		Bomb.Play ();
	}

	void ShowScore(){
		score++;
		Ttext.text = "Score：" + score;
	}


	void CamerFollow(){
		Camer.position = Vector3.MoveTowards (Camer.position, targetPos, 3f*Time.deltaTime);
	}

	void ReloadScene(){
		SceneManager.LoadScene ("tiaotiao", LoadSceneMode.Single);
	}

	void CheckGameOver(){
		if (isGameOver) {
			Invoke ("ReloadScene", 1f);
			return;
		}

		//重新加载游戏
		if (transform.position.y < -2f){
			ReloadScene ();
		}
	}

}
