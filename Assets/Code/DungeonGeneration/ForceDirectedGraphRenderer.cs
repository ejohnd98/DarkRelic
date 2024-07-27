using System;
using System.Collections.Generic;
using EpForceDirectedGraph.cs;
using UnityEngine;
using UnityEngine.UIElements;

class ForceDirectedGraphRenderer: AbstractRenderer
{
   public Transform parentTransform;
   private Color visColor = Color.white;

   public List<GameObject> nodeObjs = new();
   public List<GameObject> edgeObjs = new();

   public ForceDirectedGraphRenderer(IForceDirected iForceDirected): base(iForceDirected)
   {
      // Your initialization to draw
   }

   public override void Clear()
   {
      // Clear previous drawing if needed
      // will be called when AbstractRenderer:Draw is called
      foreach (var node in nodeObjs){
         GameObject.Destroy(node);
      }
      foreach (var edge in edgeObjs){
         GameObject.Destroy(edge);
      }
      nodeObjs.Clear();
      edgeObjs.Clear();
   }
   
   protected override void drawEdge(Edge iEdge, AbstractVector iPosition1, AbstractVector iPosition2)
   {
      // Draw the given edge according to given positions
      Vector3 position1 = new Vector3(iPosition1.x, iPosition1.y, iPosition1.z);
      Vector3 position2 = new Vector3(iPosition2.x, iPosition2.y, iPosition2.z);

      var edgePos = (position1 + position2) * 0.5f;
      var edgeScale = new Vector3(1.0f, 1.0f, (position1 - position2).magnitude);

      GameObject edgeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
      edgeObj.transform.parent = parentTransform;
      edgeObj.GetComponent<BoxCollider>().enabled = false;
      edgeObj.transform.localPosition = edgePos;
      edgeObj.transform.localScale = edgeScale;
      edgeObj.transform.rotation = Quaternion.LookRotation (position2 - position1);
      edgeObj.name = iEdge.Data.label;
      edgeObjs.Add(edgeObj);

      edgeObj.GetComponent<MeshRenderer>().material.color = visColor;
   }

   protected override void drawNode(Node iNode, AbstractVector iPosition)
   {
      // Draw the given node according to given position
      GameObject nodeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
      nodeObj.transform.parent = parentTransform;
      nodeObj.GetComponent<BoxCollider>().enabled = false;
      nodeObj.transform.localPosition = new Vector3(iPosition.x, iPosition.y, iPosition.z);
      nodeObj.transform.localScale = new Vector3(10.0f, 10.0f, 1.0f);
      nodeObj.name = iNode.Data.label;
      nodeObjs.Add(nodeObj);

      nodeObj.GetComponent<MeshRenderer>().material.color = visColor;
   }

   public void SetColor(Color color)
   {
      visColor = color;
   }
};