using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Boost : DropItem
{
    const float shadowFadeoutTime = 0.3f;
    const float secondsBetweenShadowDrops = 0.05f;
    const float shadowStartAlpha = 0.7f;
    float timeUntilShadowDrop = 0f;

    bool deactivated = false;

    public override bool TryDrop(Player p) {
        base.TryDrop(p);

        if (p.currentBoost > 1f) {
            return false;
        }

        lifeTime = 4f;
        lifeTime += 1f; //to account for fadeout
		AudioManager.Main.PlayNewSound ("Boost");
        return true;
    }

    public new void Update() {
        base.Update();

        if (deactivated) {
            return;
        }
        owner.currentBoost = 2f;
        transform.position = owner.transform.position;

        timeUntilShadowDrop -= Time.deltaTime;
        if (timeUntilShadowDrop < 0) {
            StartCoroutine(DropShadow());
            timeUntilShadowDrop = secondsBetweenShadowDrops;
        }
    }

    IEnumerator DropShadow() {
        GameObject shadow = Instantiate(owner.gameObject) as GameObject;
        shadow.GetComponents<Behaviour>()
              .ToList()
              .ForEach(c => c.enabled = false);
        shadow.GetComponent<Collider2D>().enabled = false;
        shadow.GetComponent<Rigidbody2D>().isKinematic = true;

        const int fadeOutSmoothness = 10;
        for (int i=fadeOutSmoothness; i>=0; i--) {
            Color fadedColor = shadow.GetComponent<SpriteRenderer>().color;
            fadedColor.a = shadowStartAlpha * ((float)i / (float)fadeOutSmoothness);
            shadow.GetComponent<SpriteRenderer>().color = fadedColor;
            yield return new WaitForSeconds(shadowFadeoutTime / fadeOutSmoothness);
        }
        Destroy(shadow);
    }

    public override void Deactivate() {
        owner.currentBoost = 1f;
        deactivated = true;
        Destroy(gameObject, 1f);
    }
}
