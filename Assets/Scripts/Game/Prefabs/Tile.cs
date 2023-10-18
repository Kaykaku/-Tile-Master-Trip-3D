using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DG.Tweening;

namespace Game
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Outline outline;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Collider coll;
        [SerializeField] private MeshRenderer meshRen;
        [SerializeField] private TileType type;

        private Vector3 rotation = Vector3.zero;
        private float moveToSlotTime = 0.5f;
        private float scaleInSlot = 0.5f;
        private float windForce = 7f;
        private float windRotation = 2f;
        private float lockTime = 0.1f;

        public TileType Type { get => type;}

        public void Init(TileType type , Sprite sprite , TileConfig tileConfig)
        {
            this.type = type;
            moveToSlotTime = tileConfig.MoveToSlotTime;
            scaleInSlot = tileConfig.ScaleInSlot;
            windForce = tileConfig.WindForce;
            windRotation = tileConfig.WindRotation;
            lockTime = tileConfig.LockTime;

            Material temp = Instantiate(meshRen.material);
            temp.mainTexture = sprite.texture;
            meshRen.material = temp;
        }

        void Start()
        {
            StartCoroutine(LockRotation());
        }

        void FixedUpdate()
        {
            Vector3 objectDirection = transform.up;

            if (objectDirection.y < 0.5f)
            {
                objectDirection.y = 0.5f;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(objectDirection), Time.fixedDeltaTime * 2f);
            }
        }
        public void Select(bool isSelect)
        {
            outline.enabled = isSelect;
        }
        public void AddWind()
        {
            rb.AddForce(new Vector3(Random.Range(-windRotation, windRotation), Random.Range(0, windForce), Random.Range(-windRotation, windRotation)), ForceMode.Impulse);
        }
        private IEnumerator LockRotation()
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            yield return new WaitForSeconds(lockTime);
            rb.constraints = RigidbodyConstraints.None;
        }
        public void MoveToSlot(Transform slotTf)
        {
            rb.isKinematic = true;
            coll.enabled = false;
            transform.DOMove(slotTf.position + Vector3.up, moveToSlotTime);
            transform.DORotate(rotation, moveToSlotTime);
            transform.DOScale(Vector3.one * scaleInSlot, moveToSlotTime);
        }
        public void ReleaseFromSlot(Vector3 pos)
        {
            rb.isKinematic = false;
            coll.enabled = true;
            transform.DOMove(pos, moveToSlotTime);
            transform.DOScale(Vector3.one, moveToSlotTime);
        }
    }
}