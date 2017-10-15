// Copyright (C) 2017 The Regents of the University of California (Regents).
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//
//     * Redistributions in binary form must reproduce the above
//       copyright notice, this list of conditions and the following
//       disclaimer in the documentation and/or other materials provided
//       with the distribution.
//
//     * Neither the name of The Regents or University of California nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDERS OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.
//
// Please contact the author of this library if you have any questions.
// Author: Nikolas Chaconas (nikolas.chaconas@gmail.com)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class TwoHandedArrowsManager {
	private static TwoHandedGesturesManager handsManager = null;
	private Vector3 leftArrowPositionBegin;
	private Vector3 leftArrowPositionEnd;
	private Vector3 rightArrowPositionBegin;
	private Vector3 rightArrowPositionEnd;
	private LineRenderer leftLineRenderer;
	private LineRenderer rotationAxisRenderer;
	public LineRenderer rotationAxisRenderer2;
	private LineRenderer rightLineRenderer;

	public TwoHandedArrowsManager(TwoHandedGesturesManager _handsManager) {
		handsManager = _handsManager;
	}

	public static Vector3 GetBoundingCornerPosition(float x, float y, float z, GameObject boundingCube) {
		if (!boundingCube)
			return new Vector3(0,0,0);
		Vector3 corner;
		Vector3 center;

		center = boundingCube.transform.position;

		corner.x = x;
		corner.y = y;
		corner.z = z;

		//rotate 180
		Quaternion rotate180 = new Quaternion();
		rotate180.eulerAngles = new Vector3(0,180,0);
		corner = rotate180 * corner;

		//rotate to match bounding cube
		corner = boundingCube.transform.rotation * corner;

		//translate
		corner = corner + center;

		return corner;
	}

    public void enableRotationImage()
    {
        Vector3 imagePosition = GetBoundingCornerPosition(0, handsManager.initialBoundingHeight, -handsManager.initialBoundingLength, handsManager.boundingCube);

        handsManager.yawImage.SetActive(false);
        handsManager.pitchImage.SetActive(false);
        handsManager.rollImage.SetActive(false);
        handsManager.yawImage2.SetActive(false);
        handsManager.pitchImage2.SetActive(false);
        handsManager.rollImage2.SetActive(false);
        handsManager.resizeImage.SetActive(false);

        handsManager.technique2Image.SetActive(false);


        handsManager.rotationImage.SetActive(true);
        float extraY = handsManager.rotationImage.transform.localScale.y;
        Vector3 pos = imagePosition;
        pos.y += extraY;

        handsManager.MoveObject(handsManager.rotationImage, pos);
        handsManager.rotationImage.transform.LookAt(2 * pos - Camera.main.transform.position);

        Vector3 begin = GetBoundingCornerPosition(.003f, 0.015f, .06f, handsManager.rotationImage);
        Vector3 end = GetBoundingCornerPosition(.003f, 0.03f, -.06f, handsManager.rotationImage);
        PositionLine(rotationAxisRenderer2, begin, end);
        rotationAxisRenderer2.enabled = true;
        rotationAxisRenderer.enabled = false;
        Transform cubeTransform = handsManager.rotationImage.transform.GetChild(0);
        if (TwoHandedGesturesManager.resizing || TwoHandedGesturesManager.rotating)
        {


            Vector3 cubePos = cubeTransform.position;
            cubePos.x = handsManager.rotationImage.GetComponent<RectTransform>().rect.width / 3 * handsManager.rotationImage.transform.localScale.x;
            int multiplier;

            cubeTransform.gameObject.GetComponent<Renderer>().enabled = true;
            if (handsManager.rotationType != -1)
            {
                switch (handsManager.rotationType)
                {
                    case TwoHandedRotationManager.pitchRotation:
                        multiplier = 1;
                        break;
                    case TwoHandedRotationManager.yawRotation:
                        multiplier = -1;
                        break;
                    default:
                        multiplier = 0;
                        break;
                }
            }
            else
            {
                cubeTransform.gameObject.GetComponent<Renderer>().enabled = false;
                return;
            }
            cubePos.x *= multiplier;
            cubeTransform.position = cubePos;
        } else
        {
             cubeTransform.gameObject.GetComponent<Renderer>().enabled = false;
        }
    }

	public void enableTechnique2Image() {

		Vector3 imagePosition = GetBoundingCornerPosition (0, handsManager.initialBoundingHeight, -handsManager.initialBoundingLength, handsManager.boundingCube);
		float technique2ImageHeight = handsManager.technique2Image.GetComponent<RectTransform>().rect.height * handsManager.technique2Image.transform.localScale.x;
		Vector3 technique2ImagePosition = imagePosition;

		technique2ImagePosition.y += technique2ImageHeight;
		handsManager.MoveObject (handsManager.technique2Image, technique2ImagePosition);
		handsManager.technique2Image.transform.LookAt (2 * technique2ImagePosition - Camera.main.transform.position);
		handsManager.technique2Image.SetActive(true);

		Vector3 begin = GetBoundingCornerPosition (.07f, 0.025f, .06f, handsManager.technique2Image);
		Vector3 end = GetBoundingCornerPosition (.07f, 0.04f, -.06f, handsManager.technique2Image);

		PositionLine (rotationAxisRenderer2, begin, end);
		rotationAxisRenderer2.enabled = true;

        Transform cubeTransform = handsManager.technique2Image.transform.GetChild(0);

        Vector3 cubePos = cubeTransform.position;
        cubePos.x = handsManager.technique2Image.GetComponent<RectTransform>().rect.width / 8 * handsManager.technique2Image.transform.localScale.x;
        int multiplier;

        if(InteractionManager.numSourceStates == 0)
        {
            cubeTransform.gameObject.GetComponent<Renderer>().enabled = false;
        } else
        {
            cubeTransform.gameObject.GetComponent<Renderer>().enabled = true;
            if (handsManager.rotationType != -1)
            {
                switch (handsManager.rotationType)
                {
                    case TwoHandedRotationManager.pitchRotation:
                        multiplier = 1;
                        break;
                    case TwoHandedRotationManager.yawRotation:
                        multiplier = -3;
                        break;
                    default:
                        multiplier = -1;
                        break;
                }
            }
            else
            {
                multiplier = 3;
            }
            cubePos.x *= multiplier;
            cubeTransform.position = cubePos;
        }


	}

	internal void HideArrows() {
		leftLineRenderer.enabled = false;
		rightLineRenderer.enabled = false;
		rotationAxisRenderer.enabled = false;
		handsManager.pitchImage.SetActive(false);
		handsManager.yawImage.SetActive(false);
		handsManager.rollImage.SetActive(false);
		handsManager.pitchImage2.SetActive(false);
		handsManager.yawImage2.SetActive(false);
		handsManager.rollImage2.SetActive(false);
		handsManager.rotationImage.SetActive(false);
		handsManager.resizeImage.SetActive(false);
		handsManager.HideObject(handsManager.leftTriangle);
		handsManager.HideObject(handsManager.rightTriangle);
		handsManager.HideObject(handsManager.leftDot);
		handsManager.HideObject(handsManager.rightDot);
		handsManager.boundingSphere.SetActive(false);
	}

	internal void InitializeRotationAxis() {
		const float axisWidth = .0022f;

		Material mat = new Material (handsManager.arrowMaterial);

		rotationAxisRenderer = handsManager.rotationAxis.AddComponent<LineRenderer>();
		rotationAxisRenderer.material = mat;
		rotationAxisRenderer.startWidth = axisWidth;
		rotationAxisRenderer.endWidth = axisWidth;
		rotationAxisRenderer.positionCount = 2;

		rotationAxisRenderer2 = handsManager.rotationAxis2.AddComponent<LineRenderer>();
		rotationAxisRenderer2.material = mat;
		rotationAxisRenderer2.startWidth = axisWidth;
		rotationAxisRenderer2.endWidth = axisWidth;
		rotationAxisRenderer2.positionCount = 2;

		handsManager.ColorObject (handsManager.rotationAxis, new Color(230, 230, 230));
		handsManager.ColorObject (handsManager.rotationAxis2, new Color(230, 230, 230));
	}

	internal void setPitchAxisImages(Vector3 imagePosition, Vector3 center, float width, float height, float length) {
		Vector3 begin;
		Vector3 end;
		float distance;
		float axisLength;
		Vector3 imagePosition2;
		float imageHeight = handsManager.yawImage.transform.localScale.y * 0.4f;

		handsManager.yawImage.SetActive(false);
		handsManager.rollImage.SetActive(false);
		handsManager.yawImage2.SetActive(false);
		handsManager.rollImage2.SetActive(false);
		handsManager.resizeImage.SetActive(false);

        if (TwoHandedGesturesManager.TECHNIQUE_SELECTED != TwoHandedGesturesManager.TECHNIQUE_1)
        {
            handsManager.rotationImage.SetActive(false);
        }
        else
        {
            enableRotationImage();
        }

        handsManager.pitchImage.SetActive(true);
		handsManager.pitchImage2.SetActive(true);




        begin = center;
		end = begin + handsManager.axisForRotation;
		begin -= handsManager.axisForRotation;

		axisLength = width*2;
		distance = Vector3.Distance(begin, center);
		begin = Vector3.MoveTowards(begin, center, distance - axisLength/2);
		distance = Vector3.Distance(end, center);
		end = Vector3.MoveTowards(end, center, distance - axisLength/2);

		imagePosition2 = Vector3.MoveTowards(begin, center, -imageHeight * 0.5f);
		imagePosition = Vector3.MoveTowards(end, center, -imageHeight * 0.5f);
		imagePosition.y += .0011f;
		imagePosition2.y += .0011f;

		handsManager.MoveObject(handsManager.pitchImage, imagePosition);
		handsManager.pitchImage.transform.LookAt (2 * imagePosition - Camera.main.transform.position);

		handsManager.MoveObject(handsManager.pitchImage2, imagePosition2);
		handsManager.pitchImage2.transform.LookAt (2 * imagePosition2 - Camera.main.transform.position);

		PositionLine (rotationAxisRenderer, begin, end);
	}

	internal void setYawAxisImages(Vector3 imagePosition, Vector3 center, float width, float height, float length) {
		Vector3 begin;
		Vector3 end;
		float distance;
		float axisLength;
		Vector3 imagePosition2;
		float imageHeight = handsManager.yawImage.transform.localScale.y * 0.4f;

		handsManager.pitchImage.SetActive (false);
		handsManager.rollImage.SetActive (false);
		handsManager.pitchImage2.SetActive (false);
		handsManager.rollImage2.SetActive (false);
        if (TwoHandedGesturesManager.TECHNIQUE_SELECTED != TwoHandedGesturesManager.TECHNIQUE_1)
        {
            handsManager.rotationImage.SetActive(false);
        }
        else
        {
			enableRotationImage();
        }
		handsManager.resizeImage.SetActive(false);
		handsManager.yawImage.SetActive(true);
		handsManager.yawImage2.SetActive(true);

		begin = center;
		end = begin + handsManager.axisForRotation;
		begin -= handsManager.axisForRotation;

		axisLength = height*2;
		distance = Vector3.Distance(begin, center);
		begin = Vector3.MoveTowards(begin, center, distance - axisLength/2);
		distance = Vector3.Distance(end, center);
		end = Vector3.MoveTowards(end, center, distance - axisLength/2);

		imagePosition = end;
		imagePosition.y += imageHeight * 0.5f;

		imagePosition2 = begin;
		imagePosition2.y -= imageHeight * 0.5f;

		handsManager.MoveObject(handsManager.yawImage, imagePosition);
		handsManager.yawImage.transform.LookAt (2 * imagePosition - Camera.main.transform.position);

		handsManager.MoveObject(handsManager.yawImage2, imagePosition2);
		handsManager.yawImage2.transform.LookAt (2 * imagePosition2 - Camera.main.transform.position);

		PositionLine (rotationAxisRenderer, begin, end);
	}

	internal void setRollAxisImages(Vector3 imagePosition, Vector3 center, float width, float height, float length) {
		Vector3 begin;
		Vector3 end;
		float distance;
		float axisLength;
		Vector3 imagePosition2;
		float amountTowards = 0.1f;

		handsManager.yawImage.SetActive (false);
		handsManager.pitchImage.SetActive (false);
		handsManager.yawImage2.SetActive (false);
		handsManager.pitchImage2.SetActive (false);
        if (TwoHandedGesturesManager.TECHNIQUE_SELECTED != TwoHandedGesturesManager.TECHNIQUE_1)
        {
            handsManager.rotationImage.SetActive(false);
        }
        else
        {
            enableRotationImage();
        }
        handsManager.resizeImage.SetActive(false);
		handsManager.rollImage.SetActive (true);
		handsManager.rollImage2.SetActive (false);
		begin = center;
		begin.y -= amountTowards;
		end = center + handsManager.axisForRotation;
		end.y += amountTowards;
		begin -= handsManager.axisForRotation;

		axisLength = length*4;
		distance = Vector3.Distance(begin, center);
		begin = Vector3.MoveTowards(begin, center, distance - axisLength/2);
		distance = Vector3.Distance(end, center);
		end = Vector3.MoveTowards(end, center, distance - axisLength/2);

		imagePosition = Vector3.MoveTowards(begin, center, .02f);
		imagePosition2 = Vector3.MoveTowards(end, center, .02f);

		handsManager.MoveObject(handsManager.rollImage, imagePosition);
		handsManager.rollImage.transform.LookAt (2 * imagePosition - Camera.main.transform.position);

		handsManager.MoveObject(handsManager.rollImage2, imagePosition2);
		handsManager.rollImage2.transform.LookAt (2 * imagePosition2 - Camera.main.transform.position);

		PositionLine (rotationAxisRenderer, begin, end);
	}

	internal void InitializeArrows() {
		const float arrowWidth = .003f;

		Material leftMaterial = new Material (handsManager.arrowMaterial);
		Material rightMaterial = new Material (handsManager.arrowMaterial);

		leftLineRenderer = handsManager.leftLine.AddComponent<LineRenderer>();
		leftLineRenderer.material = leftMaterial;
		leftLineRenderer.startWidth = arrowWidth;
		leftLineRenderer.endWidth = arrowWidth;
		leftLineRenderer.positionCount = 2;

		handsManager.leftTriangle.GetComponent<MeshFilter>().mesh = new Mesh();
        handsManager.leftTriangle.GetComponent<Renderer>().material = leftMaterial;

		rightLineRenderer = handsManager.rightLine.AddComponent<LineRenderer>();
		rightLineRenderer.material = rightMaterial;
		rightLineRenderer.startWidth = arrowWidth;
		rightLineRenderer.endWidth = arrowWidth;
		rightLineRenderer.positionCount = 2;

		handsManager.rightTriangle.GetComponent<MeshFilter>().mesh = new Mesh();
		handsManager.rightTriangle.GetComponent<Renderer>().material = rightMaterial;
	}


	public void PositionLine(LineRenderer line, Vector3 begin, Vector3 end) {
		var points = new Vector3[2];
		points [0] = begin;
		points [1] = end;

		line.SetPositions(points);
	}

	public void PositionArrows(int type, float width, float length, float height) {
		float arrowLength = .008f;
		float endWidth = width + arrowLength;
		float endHeight = height + arrowLength;
		//yaw arrow length
		float amount = arrowLength + .03f;
		float amount2 = arrowLength - .003f;
		float triangleWidthLarge = endWidth + .005f;
		float triangleHeightLarge = endHeight + .005f;
		float triangleWidthSmall = width + amount2;
		float triangleHeightSmall = height + amount2;
		float rotWidthLarge = triangleWidthSmall;
		float rotWidthSmall = width  - amount2;
		float rotHeight = height/6 + amount2;
		float zAxisAmount = .06f;
		float yAxisAmount = height;

		Vector3 leftTrianglePosition1;
		Vector3 leftTrianglePosition2;
		Vector3 leftTrianglePosition3;

		Vector3 rightTrianglePosition1;
		Vector3 rightTrianglePosition2;
		Vector3 rightTrianglePosition3;

		rightLineRenderer.enabled = true;
		leftLineRenderer.enabled = true;
		Vector3 imagePosition;

		float bufferSpace = 0.0f;

		imagePosition = GetBoundingCornerPosition (0, height + handsManager.yawImage.transform.localScale.y/4 + bufferSpace, -length, handsManager.boundingCube);

		if (handsManager.rotationType != -1) {
			setRotationAxis();
		}

		switch(type) {
			case TwoHandedGesturesManager.Horizontal:
				switch (handsManager.rotationType) {
					case TwoHandedRotationManager.pitchRotation:
						if(TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_1) {
							if(handsManager.rightHandPosition.y < handsManager.initialRight.y) {
								//set left line
								leftArrowPositionBegin = GetBoundingCornerPosition (-width, -height/6, -length, handsManager.boundingCube);
								leftArrowPositionEnd = GetBoundingCornerPosition (-width, height/6, -length, handsManager.boundingCube);
								handsManager.MoveObject(handsManager.leftDot, leftArrowPositionBegin);

								//set left triangle
								leftTrianglePosition1 = GetBoundingCornerPosition (-rotWidthLarge, height/6, -length, handsManager.boundingCube);
								leftTrianglePosition2 = GetBoundingCornerPosition (-width, rotHeight, -length, handsManager.boundingCube);
								leftTrianglePosition3 = GetBoundingCornerPosition (-rotWidthSmall, height/6, -length, handsManager.boundingCube);

								//set right line
								rightArrowPositionBegin = GetBoundingCornerPosition (width, -height/6, -length, handsManager.boundingCube);
								rightArrowPositionEnd = GetBoundingCornerPosition (width, height/6, -length, handsManager.boundingCube);
								handsManager.MoveObject(handsManager.rightDot, rightArrowPositionBegin);

								//set right triangle
								rightTrianglePosition1 = GetBoundingCornerPosition (rotWidthLarge, height/6, -length, handsManager.boundingCube);
								rightTrianglePosition2 = GetBoundingCornerPosition (rotWidthSmall, height/6, -length, handsManager.boundingCube);
								rightTrianglePosition3 = GetBoundingCornerPosition (width, rotHeight, -length, handsManager.boundingCube);
							} else {
								//set left line
								leftArrowPositionBegin = GetBoundingCornerPosition (-width, -height/6, -length, handsManager.boundingCube);
								leftArrowPositionEnd = GetBoundingCornerPosition (-width, height/6, -length, handsManager.boundingCube);
								handsManager.MoveObject(handsManager.leftDot, leftArrowPositionEnd);

								//set left triangle
								leftTrianglePosition1 = GetBoundingCornerPosition (-rotWidthLarge, -height/6, -length, handsManager.boundingCube);
								leftTrianglePosition2 = GetBoundingCornerPosition (-rotWidthSmall, -height/6, -length, handsManager.boundingCube);
								leftTrianglePosition3 = GetBoundingCornerPosition (-width, -rotHeight, -length, handsManager.boundingCube);

								//set right line
								rightArrowPositionBegin = GetBoundingCornerPosition (width, -height/6, -length, handsManager.boundingCube);
								rightArrowPositionEnd = GetBoundingCornerPosition (width, height/6, -length, handsManager.boundingCube);
								handsManager.MoveObject(handsManager.rightDot, rightArrowPositionEnd);

								//set right triangle
								rightTrianglePosition1 = GetBoundingCornerPosition (rotWidthLarge, -height/6, -length, handsManager.boundingCube);
								rightTrianglePosition2 = GetBoundingCornerPosition (width, -rotHeight, -length, handsManager.boundingCube);
								rightTrianglePosition3 = GetBoundingCornerPosition (rotWidthSmall, -height/6, -length, handsManager.boundingCube);
							}
						} else if(TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_2) {
								if(handsManager.rightHandDistance > handsManager.leftHandDistance) {
									//right hand far and high, left hand close and low
									if(handsManager.rightHandPosition.y > handsManager.leftHandPosition.y) {
										//set left line
										leftArrowPositionBegin = GetBoundingCornerPosition (-width/3, -yAxisAmount*1.3f, -length - zAxisAmount, handsManager.boundingCube);
										leftArrowPositionEnd = GetBoundingCornerPosition (-width/3, amount/3 - yAxisAmount*1.3f, -(length - amount) - zAxisAmount, handsManager.boundingCube);
										handsManager.MoveObject(handsManager.leftDot, leftArrowPositionBegin);

										//set left triangle
										leftTrianglePosition1 = GetBoundingCornerPosition (-rotWidthLarge + (2*width/3), amount/3 - yAxisAmount*1.3f, -(length - amount) - zAxisAmount, handsManager.boundingCube);
										leftTrianglePosition2 = GetBoundingCornerPosition (-width/3, amount/3 +  amount2 - yAxisAmount*1.3f, -(length - amount - amount2) - zAxisAmount, handsManager.boundingCube);
										leftTrianglePosition3 = GetBoundingCornerPosition (-rotWidthSmall + (2*width/3), amount/3 - yAxisAmount*1.3f, -(length - amount) - zAxisAmount, handsManager.boundingCube);

										//set right line
										rightArrowPositionBegin = GetBoundingCornerPosition (width/3, yAxisAmount*1.3f, -length + zAxisAmount, handsManager.boundingCube);
										rightArrowPositionEnd = GetBoundingCornerPosition (width/3, -amount/3 + yAxisAmount*1.3f, -(length + amount) + zAxisAmount, handsManager.boundingCube);
										handsManager.MoveObject(handsManager.rightDot, rightArrowPositionBegin);

										//set right triangle
										rightTrianglePosition1 = GetBoundingCornerPosition (rotWidthLarge - (2*width/3), -amount/3 + yAxisAmount*1.3f, -(length + amount) + zAxisAmount, handsManager.boundingCube);
										rightTrianglePosition2 = GetBoundingCornerPosition (width/3, -amount/3 -  amount2 + yAxisAmount*1.3f, -(length + amount + amount2) + zAxisAmount, handsManager.boundingCube);
										rightTrianglePosition3 = GetBoundingCornerPosition (rotWidthSmall - (2*width/3), -amount/3 + yAxisAmount*1.3f, -(length + amount) + zAxisAmount, handsManager.boundingCube);
									} else {
										//righthand far and low, left hand close and high
										//set left line
										leftArrowPositionBegin = GetBoundingCornerPosition (-width/3,  yAxisAmount*1.3f, -length - zAxisAmount, handsManager.boundingCube);
										leftArrowPositionEnd = GetBoundingCornerPosition (-width/3, amount/3 + yAxisAmount*1.3f, -(length - amount) - zAxisAmount, handsManager.boundingCube);
										handsManager.MoveObject(handsManager.leftDot, leftArrowPositionBegin);

										//set left triangle
										leftTrianglePosition1 = GetBoundingCornerPosition (-rotWidthLarge + (2*width/3), amount/3 + yAxisAmount*1.3f, -(length - amount) - zAxisAmount, handsManager.boundingCube);
										leftTrianglePosition2 = GetBoundingCornerPosition (-width/3, amount/3 +  amount2 + yAxisAmount*1.3f, -(length - amount - amount2) - zAxisAmount, handsManager.boundingCube);
										leftTrianglePosition3 = GetBoundingCornerPosition (-rotWidthSmall + (2*width/3), amount/3 + yAxisAmount*1.3f, -(length - amount) - zAxisAmount, handsManager.boundingCube);

										//set right line
										rightArrowPositionBegin = GetBoundingCornerPosition (width/3, - yAxisAmount*1.3f, -length + zAxisAmount, handsManager.boundingCube);
										rightArrowPositionEnd = GetBoundingCornerPosition (width/3, -amount/3 - yAxisAmount*1.3f, -(length + amount) + zAxisAmount, handsManager.boundingCube);
										handsManager.MoveObject(handsManager.rightDot, rightArrowPositionBegin);

										//set right triangle
										rightTrianglePosition1 = GetBoundingCornerPosition (rotWidthLarge - (2*width/3), -amount/3 - yAxisAmount*1.3f, -(length + amount) + zAxisAmount, handsManager.boundingCube);
										rightTrianglePosition2 = GetBoundingCornerPosition (width/3, -amount/3 -  amount2 - yAxisAmount*1.3f, -(length + amount + amount2) + zAxisAmount, handsManager.boundingCube);
										rightTrianglePosition3 = GetBoundingCornerPosition (rotWidthSmall - (2*width/3), -amount/3 - yAxisAmount*1.3f, -(length + amount) + zAxisAmount, handsManager.boundingCube);
									}
								} else {
									if(handsManager.rightHandPosition.y > handsManager.leftHandPosition.y) {
										//right hand close and high, left hand far and low
										//set left line
										leftArrowPositionBegin = GetBoundingCornerPosition (-width/3, - yAxisAmount*1.3f, -length + zAxisAmount, handsManager.boundingCube);
										leftArrowPositionEnd = GetBoundingCornerPosition (-width/3, -amount/3 - yAxisAmount*1.3f, -(length + amount) + zAxisAmount, handsManager.boundingCube);
										handsManager.MoveObject(handsManager.leftDot, leftArrowPositionBegin);

										//set left triangle
										leftTrianglePosition1 = GetBoundingCornerPosition (-rotWidthLarge + (2*width/3), -amount/3 - yAxisAmount*1.3f, -(length + amount) + zAxisAmount, handsManager.boundingCube);
										leftTrianglePosition2 = GetBoundingCornerPosition (-rotWidthSmall + (2*width/3), -amount/3 - yAxisAmount*1.3f, -(length + amount) + zAxisAmount, handsManager.boundingCube);
										leftTrianglePosition3 = GetBoundingCornerPosition (-width/3, -amount/3 -  amount2 - yAxisAmount*1.3f, -(length + amount + amount2) + zAxisAmount, handsManager.boundingCube);

										//set right line
										rightArrowPositionBegin = GetBoundingCornerPosition (width/3,  yAxisAmount*1.3f, -length - zAxisAmount, handsManager.boundingCube);
										rightArrowPositionEnd = GetBoundingCornerPosition (width/3, amount/3 + yAxisAmount*1.3f, -(length - amount) - zAxisAmount, handsManager.boundingCube);
										handsManager.MoveObject(handsManager.rightDot, rightArrowPositionBegin);

										//set right triangle
										rightTrianglePosition1 = GetBoundingCornerPosition (rotWidthLarge - (2*width/3), amount/3 + yAxisAmount*1.3f, -(length - amount) - zAxisAmount, handsManager.boundingCube);
										rightTrianglePosition2 = GetBoundingCornerPosition (rotWidthSmall - (2*width/3), amount/3 + yAxisAmount*1.3f, -(length - amount) - zAxisAmount, handsManager.boundingCube);
										rightTrianglePosition3 = GetBoundingCornerPosition (width/3, amount/3 +  amount2 + yAxisAmount*1.3f, -(length - amount - amount2) - zAxisAmount, handsManager.boundingCube);

									} else {
										//right hand close and low, left hand far and high
										//set left line
										leftArrowPositionBegin = GetBoundingCornerPosition (-width/3, yAxisAmount*1.3f, -length + zAxisAmount, handsManager.boundingCube);
										leftArrowPositionEnd = GetBoundingCornerPosition (-width/3, -amount/3 + yAxisAmount*1.3f, -(length + amount) + zAxisAmount, handsManager.boundingCube);
										handsManager.MoveObject(handsManager.leftDot, leftArrowPositionBegin);

										//set left triangle
										leftTrianglePosition1 = GetBoundingCornerPosition (-rotWidthLarge + (2*width/3), -amount/3 + yAxisAmount*1.3f, -(length + amount) + zAxisAmount, handsManager.boundingCube);
										leftTrianglePosition2 = GetBoundingCornerPosition (-rotWidthSmall + (2*width/3), -amount/3 + yAxisAmount*1.3f, -(length + amount) + zAxisAmount, handsManager.boundingCube);
										leftTrianglePosition3 = GetBoundingCornerPosition (-width/3, -amount/3 -  amount2 + yAxisAmount*1.3f, -(length + amount + amount2) + zAxisAmount, handsManager.boundingCube);

										//set right line
										rightArrowPositionBegin = GetBoundingCornerPosition (width/3, - yAxisAmount*1.3f, -length - zAxisAmount, handsManager.boundingCube);
										rightArrowPositionEnd = GetBoundingCornerPosition (width/3, amount/3 - yAxisAmount*1.3f, -(length - amount) - zAxisAmount, handsManager.boundingCube);
										handsManager.MoveObject(handsManager.rightDot, rightArrowPositionBegin);

										//set right triangle
										rightTrianglePosition1 = GetBoundingCornerPosition (rotWidthLarge - (2*width/3), amount/3 - yAxisAmount*1.3f, -(length - amount) - zAxisAmount, handsManager.boundingCube);
										rightTrianglePosition2 = GetBoundingCornerPosition (rotWidthSmall - (2*width/3), amount/3 - yAxisAmount*1.3f, -(length - amount) - zAxisAmount, handsManager.boundingCube);
										rightTrianglePosition3 = GetBoundingCornerPosition (width/3, amount/3 +  amount2 - yAxisAmount*1.3f, -(length - amount - amount2) - zAxisAmount, handsManager.boundingCube);
									}
								}
						} else {
							return;
						}
						break;
					case TwoHandedRotationManager.rollRotation:
						if(Mathf.Abs(handsManager.leftHandPosition.y - handsManager.rightHandPosition.y) < TwoHandedGesturesManager.ROTATION_THRESHOLD) {
							//set left line
							leftArrowPositionBegin = GetBoundingCornerPosition (-width, -height/6, -length, handsManager.boundingCube);
							leftArrowPositionEnd = GetBoundingCornerPosition (-width, height/6, -length, handsManager.boundingCube);
							handsManager.MoveObject(handsManager.leftDot, leftArrowPositionBegin);

							//set left triangle
							leftTrianglePosition1 = GetBoundingCornerPosition (-rotWidthLarge, height/6, -length, handsManager.boundingCube);
							leftTrianglePosition2 = GetBoundingCornerPosition (-width, rotHeight, -length, handsManager.boundingCube);
							leftTrianglePosition3 = GetBoundingCornerPosition (-rotWidthSmall, height/6, -length, handsManager.boundingCube);

							//set right line
							rightArrowPositionBegin = GetBoundingCornerPosition (width, -height/6, -length, handsManager.boundingCube);
							rightArrowPositionEnd = GetBoundingCornerPosition (width, height/6, -length, handsManager.boundingCube);
							handsManager.MoveObject(handsManager.rightDot, rightArrowPositionEnd);

							//set right triangle
							rightTrianglePosition1 = GetBoundingCornerPosition (rotWidthLarge, -height/6, -length, handsManager.boundingCube);
							rightTrianglePosition2 = GetBoundingCornerPosition (width, -rotHeight, -length, handsManager.boundingCube);
							rightTrianglePosition3 = GetBoundingCornerPosition (rotWidthSmall, -height/6, -length, handsManager.boundingCube);
						} else if(handsManager.leftHandPosition.y < handsManager.rightHandPosition.y) {
							//set left line
							leftArrowPositionBegin = GetBoundingCornerPosition (-width, -height/6 - yAxisAmount, -length, handsManager.boundingCube);
							leftArrowPositionEnd = GetBoundingCornerPosition (-width, height/6 - yAxisAmount, -length, handsManager.boundingCube);
							handsManager.MoveObject(handsManager.leftDot, leftArrowPositionBegin);

							//set left triangle
							leftTrianglePosition1 = GetBoundingCornerPosition (-rotWidthLarge, height/6 - yAxisAmount, -length, handsManager.boundingCube);
							leftTrianglePosition2 = GetBoundingCornerPosition (-width, rotHeight - yAxisAmount, -length, handsManager.boundingCube);
							leftTrianglePosition3 = GetBoundingCornerPosition (-rotWidthSmall, height/6 - yAxisAmount, -length, handsManager.boundingCube);

							//set right line
							rightArrowPositionBegin = GetBoundingCornerPosition (width, -height/6 + yAxisAmount, -length, handsManager.boundingCube);
							rightArrowPositionEnd = GetBoundingCornerPosition (width, height/6 + yAxisAmount, -length, handsManager.boundingCube);
							handsManager.MoveObject(handsManager.rightDot, rightArrowPositionEnd);

							//set right triangle
							rightTrianglePosition1 = GetBoundingCornerPosition (rotWidthLarge, -height/6 + yAxisAmount, -length, handsManager.boundingCube);
							rightTrianglePosition2 = GetBoundingCornerPosition (width, -rotHeight + yAxisAmount, -length, handsManager.boundingCube);
							rightTrianglePosition3 = GetBoundingCornerPosition (rotWidthSmall, -height/6 + yAxisAmount, -length, handsManager.boundingCube);
						} else {
							//set left line
							leftArrowPositionBegin = GetBoundingCornerPosition (-width, -height/6 + yAxisAmount, -length, handsManager.boundingCube);
							leftArrowPositionEnd = GetBoundingCornerPosition (-width, height/6 + yAxisAmount, -length, handsManager.boundingCube);
							handsManager.MoveObject(handsManager.leftDot, leftArrowPositionEnd);

							//set left triangle
							leftTrianglePosition1 = GetBoundingCornerPosition (-rotWidthLarge, -height/6 + yAxisAmount, -length, handsManager.boundingCube);
							leftTrianglePosition2 = GetBoundingCornerPosition (-rotWidthSmall, -height/6 + yAxisAmount, -length, handsManager.boundingCube);
							leftTrianglePosition3 = GetBoundingCornerPosition (-width, -rotHeight + yAxisAmount, -length, handsManager.boundingCube);


							//set right line
							rightArrowPositionBegin = GetBoundingCornerPosition (width, -height/6 - yAxisAmount, -length, handsManager.boundingCube);
							rightArrowPositionEnd = GetBoundingCornerPosition (width, height/6 - yAxisAmount, -length, handsManager.boundingCube);
							handsManager.MoveObject(handsManager.rightDot, rightArrowPositionBegin);

							//set right triangle
							rightTrianglePosition1 = GetBoundingCornerPosition (rotWidthLarge, height/6 - yAxisAmount, -length, handsManager.boundingCube);
							rightTrianglePosition2 = GetBoundingCornerPosition (rotWidthSmall, height/6 - yAxisAmount, -length, handsManager.boundingCube);
							rightTrianglePosition3 = GetBoundingCornerPosition (width, rotHeight - yAxisAmount, -length, handsManager.boundingCube);
						}
						break;
					case TwoHandedRotationManager.yawRotation:
						if(handsManager.rightHandDistance < handsManager.leftHandDistance) {
							//set left line
							leftArrowPositionBegin = GetBoundingCornerPosition (-width, amount/6, -length + zAxisAmount, handsManager.boundingCube);
							leftArrowPositionEnd = GetBoundingCornerPosition (-width, -amount/3 + amount/6, -(length + amount) + zAxisAmount, handsManager.boundingCube);
							handsManager.MoveObject(handsManager.leftDot, leftArrowPositionBegin);

							//set left triangle
							leftTrianglePosition1 = GetBoundingCornerPosition (-rotWidthLarge, -amount/3 + amount/6, -(length + amount) + zAxisAmount, handsManager.boundingCube);
							leftTrianglePosition2 = GetBoundingCornerPosition (-rotWidthSmall, -amount/3 + amount/6, -(length + amount) + zAxisAmount, handsManager.boundingCube);
							leftTrianglePosition3 = GetBoundingCornerPosition (-width, -amount/3 -  amount2 + amount/6, -(length + amount + amount2) + zAxisAmount, handsManager.boundingCube);

							//set right line
							rightArrowPositionBegin = GetBoundingCornerPosition (width, -amount/6, -length - zAxisAmount, handsManager.boundingCube);
							rightArrowPositionEnd = GetBoundingCornerPosition (width, amount/3 - amount/6, -(length - amount) - zAxisAmount, handsManager.boundingCube);
							handsManager.MoveObject(handsManager.rightDot, rightArrowPositionBegin);

							//set right triangle
							rightTrianglePosition1 = GetBoundingCornerPosition (rotWidthLarge, amount/3 - amount/6, -(length - amount) - zAxisAmount, handsManager.boundingCube);
							rightTrianglePosition2 = GetBoundingCornerPosition (rotWidthSmall, amount/3 - amount/6, -(length - amount) - zAxisAmount, handsManager.boundingCube);
							rightTrianglePosition3 = GetBoundingCornerPosition (width, amount/3 +  amount2 - amount/6, -(length - amount - amount2) - zAxisAmount, handsManager.boundingCube);
						} else {
							//set left line
							leftArrowPositionBegin = GetBoundingCornerPosition (-width, -amount/6, -length - zAxisAmount, handsManager.boundingCube);
							leftArrowPositionEnd = GetBoundingCornerPosition (-width, amount/3 - amount/6, -(length - amount) - zAxisAmount, handsManager.boundingCube);
							handsManager.MoveObject(handsManager.leftDot, leftArrowPositionBegin);

							//set left triangle
							leftTrianglePosition1 = GetBoundingCornerPosition (-rotWidthLarge, amount/3 - amount/6, -(length - amount) - zAxisAmount, handsManager.boundingCube);
							leftTrianglePosition2 = GetBoundingCornerPosition (-width, amount/3 +  amount2 - amount/6, -(length - amount - amount2) - zAxisAmount, handsManager.boundingCube);
							leftTrianglePosition3 = GetBoundingCornerPosition (-rotWidthSmall, amount/3 - amount/6, -(length - amount) - zAxisAmount, handsManager.boundingCube);

							//set right line
							rightArrowPositionBegin = GetBoundingCornerPosition (width, amount/6, -length + zAxisAmount, handsManager.boundingCube);
							rightArrowPositionEnd = GetBoundingCornerPosition (width, -amount/3 + amount/6, -(length + amount) + zAxisAmount, handsManager.boundingCube);
							handsManager.MoveObject(handsManager.rightDot, rightArrowPositionBegin);

							//set right triangle
							rightTrianglePosition1 = GetBoundingCornerPosition (rotWidthLarge, -amount/3 + amount/6, -(length + amount) + zAxisAmount, handsManager.boundingCube);
							rightTrianglePosition2 = GetBoundingCornerPosition (width, -amount/3 -  amount2 + amount/6, -(length + amount + amount2) + zAxisAmount, handsManager.boundingCube);
							rightTrianglePosition3 = GetBoundingCornerPosition (rotWidthSmall, -amount/3 + amount/6, -(length + amount) + zAxisAmount, handsManager.boundingCube);
						}
						break;
					case TwoHandedRotationManager.spindleRotation:
						handsManager.ShowObject (handsManager.leftDot);
						handsManager.ShowObject (handsManager.rightDot);

						handsManager.MoveObject(handsManager.leftDot, handsManager.leftHandSpindlePosition);
						handsManager.MoveObject(handsManager.rightDot, handsManager.rightHandSpindlePosition);


						leftArrowPositionBegin = handsManager.leftHandSpindlePosition;
						leftArrowPositionEnd = handsManager.objCenter;

						rightArrowPositionBegin = handsManager.rightHandSpindlePosition;
						rightArrowPositionEnd = handsManager.objCenter;

						PositionLine (leftLineRenderer, leftArrowPositionBegin, leftArrowPositionEnd);
						PositionLine (rightLineRenderer, rightArrowPositionBegin, rightArrowPositionEnd);

						handsManager.HideObject (handsManager.leftTriangle);
						handsManager.HideObject (handsManager.rightTriangle);
						handsManager.yawImage.SetActive (false);
						handsManager.pitchImage.SetActive (false);
						handsManager.rollImage.SetActive (false);
						handsManager.yawImage2.SetActive (false);
						handsManager.pitchImage2.SetActive (false);
						handsManager.rollImage2.SetActive (false);
						handsManager.resizeImage.SetActive(false);
						handsManager.rotationImage.SetActive(false);
						handsManager.technique2Image.SetActive(false);
						return;
					case TwoHandedRotationManager.arcBallRotation:
						if(handsManager.handBeingTracked == handsManager.leftID) {
							handsManager.ShowObject (handsManager.leftDot);
							handsManager.HideObject (handsManager.rightDot);
							handsManager.MoveObject(handsManager.leftDot, handsManager.arcBallHandPosition);
						}
						else {
							handsManager.ShowObject (handsManager.rightDot);
							handsManager.HideObject (handsManager.leftDot);
							handsManager.MoveObject(handsManager.rightDot, handsManager.arcBallHandPosition);
						}

						leftLineRenderer.enabled = false;
						rightLineRenderer.enabled = false;
						handsManager.HideObject (handsManager.leftTriangle);
						handsManager.HideObject (handsManager.rightTriangle);
						handsManager.yawImage.SetActive (false);
						handsManager.pitchImage.SetActive (false);
						handsManager.rollImage.SetActive (false);
						handsManager.yawImage2.SetActive (false);
						handsManager.pitchImage2.SetActive (false);
						handsManager.rollImage2.SetActive (false);
						handsManager.resizeImage.SetActive(false);
						handsManager.rotationImage.SetActive(false);
						handsManager.technique2Image.SetActive(false);
						return;
					case TwoHandedRotationManager.selectionRotation:
						if(handsManager.handBeingTracked == handsManager.leftID) {
							handsManager.ShowObject (handsManager.leftDot);
							handsManager.HideObject (handsManager.rightDot);
							handsManager.MoveObject(handsManager.leftDot, handsManager.arcBallHandPosition);
						}
						else {
							handsManager.ShowObject (handsManager.rightDot);
							handsManager.HideObject (handsManager.leftDot);
							handsManager.MoveObject(handsManager.rightDot, handsManager.arcBallHandPosition);
						}

						Vector3 center = handsManager.objCenter;
						switch(handsManager.technique5Selection) {
							case 0:
								setPitchAxisImages(imagePosition, center, width, height, length);
							break;
							case 1:
								setYawAxisImages(imagePosition, center, width, height, length);
							break;
							default:
								setRollAxisImages(imagePosition, center, width, height, length);
							break;
						}

						rotationAxisRenderer.enabled = true;

						leftLineRenderer.enabled = false;
						rightLineRenderer.enabled = false;
						handsManager.HideObject (handsManager.leftTriangle);
						handsManager.HideObject (handsManager.rightTriangle);
						// handsManager.yawImage.SetActive (false);
						// handsManager.pitchImage.SetActive (false);
						// handsManager.rollImage.SetActive (false);
						// handsManager.yawImage2.SetActive (false);
						// handsManager.pitchImage2.SetActive (false);
						// handsManager.rollImage2.SetActive (false);
						// handsManager.resizeImage.SetActive(false);
						// handsManager.rotationImage.SetActive(false);
						// handsManager.technique2Image.SetActive(false);
						return;
					default:
						//technique 4 resize position
						if(TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_4) {
							handsManager.HideObject (handsManager.leftDot);
							handsManager.HideObject (handsManager.rightDot);

							leftArrowPositionBegin = handsManager.leftHandSpindlePosition;
							leftArrowPositionEnd = Vector3.MoveTowards(leftArrowPositionBegin, handsManager.objCenter, -.02f);

							rightArrowPositionBegin = handsManager.rightHandSpindlePosition;
							rightArrowPositionEnd = Vector3.MoveTowards(rightArrowPositionBegin, handsManager.objCenter, -.02f);

							handsManager.ShowObject (handsManager.leftDot);
							handsManager.ShowObject (handsManager.rightDot);
							handsManager.MoveObject(handsManager.leftDot, leftArrowPositionEnd);
							handsManager.MoveObject(handsManager.rightDot, rightArrowPositionEnd);

							PositionLine (leftLineRenderer, leftArrowPositionBegin, leftArrowPositionEnd);
							PositionLine (rightLineRenderer, rightArrowPositionBegin, rightArrowPositionEnd);

							handsManager.yawImage.SetActive (false);
							handsManager.pitchImage.SetActive (false);
							handsManager.rollImage.SetActive (false);
							handsManager.yawImage2.SetActive (false);
							handsManager.pitchImage2.SetActive (false);
							handsManager.rollImage2.SetActive (false);
							handsManager.resizeImage.SetActive(true);
							handsManager.rotationImage.SetActive(false);
							handsManager.technique2Image.SetActive(false);
							handsManager.MoveObject(handsManager.resizeImage, imagePosition);
							handsManager.resizeImage.transform.LookAt (2 * imagePosition - Camera.main.transform.position);
						} else if (TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_6) {
							if(handsManager.handBeingTracked == handsManager.leftID) {
								leftArrowPositionBegin = handsManager.arcBallHandPosition;
								leftArrowPositionEnd = Vector3.MoveTowards(leftArrowPositionBegin, handsManager.objCenter, -.02f);
								handsManager.MoveObject(handsManager.leftDot, leftArrowPositionEnd);
								handsManager.ShowObject (handsManager.leftDot);
								PositionLine (leftLineRenderer, leftArrowPositionBegin, leftArrowPositionEnd);
								leftLineRenderer.enabled = true;
								rightLineRenderer.enabled = false;
								handsManager.ShowObject (handsManager.leftDot);
								handsManager.HideObject (handsManager.rightDot);

							} else {
								rightArrowPositionBegin = handsManager.arcBallHandPosition;
								rightArrowPositionEnd = Vector3.MoveTowards(rightArrowPositionBegin, handsManager.objCenter, -.02f);
								handsManager.ShowObject (handsManager.rightDot);
								handsManager.MoveObject(handsManager.rightDot, rightArrowPositionEnd);
								PositionLine (rightLineRenderer, rightArrowPositionBegin, rightArrowPositionEnd);
								leftLineRenderer.enabled = false;
								rightLineRenderer.enabled = true;
								handsManager.HideObject (handsManager.leftDot);
								handsManager.ShowObject (handsManager.rightDot);
							}

							handsManager.yawImage.SetActive (false);
							handsManager.pitchImage.SetActive (false);
							handsManager.rollImage.SetActive (false);
							handsManager.yawImage2.SetActive (false);
							handsManager.pitchImage2.SetActive (false);
							handsManager.rollImage2.SetActive (false);
							handsManager.resizeImage.SetActive(true);
							handsManager.rotationImage.SetActive(false);
							handsManager.technique2Image.SetActive(false);
							handsManager.MoveObject(handsManager.resizeImage, imagePosition);
							handsManager.resizeImage.transform.LookAt (2 * imagePosition - Camera.main.transform.position);
							rotationAxisRenderer.enabled = false;
							return;
						} else {
							//Technique 1 ready position
								//set left line
							leftArrowPositionEnd = GetBoundingCornerPosition (-(endWidth + arrowLength), 0, -length, handsManager.boundingCube);

									//set right line
							rightArrowPositionEnd = GetBoundingCornerPosition ((endWidth + arrowLength), 0, -length, handsManager.boundingCube);

							handsManager.HideObject (handsManager.leftTriangle);
							handsManager.HideObject (handsManager.rightTriangle);

							handsManager.ShowObject (handsManager.leftDot);
							handsManager.ShowObject (handsManager.rightDot);

							handsManager.MoveObject(handsManager.leftDot, leftArrowPositionEnd);
							handsManager.MoveObject(handsManager.rightDot, rightArrowPositionEnd);

							rightLineRenderer.enabled = false;
							leftLineRenderer.enabled = false;

                            enableRotationImage();
						}
						return;
				}
				break;
			case TwoHandedGesturesManager.LeftUp:
					handsManager.rotationType = -1;
					//set left line
					leftArrowPositionBegin = GetBoundingCornerPosition (-width, height, -length, handsManager.boundingCube);
					leftArrowPositionEnd = GetBoundingCornerPosition (-endWidth, endHeight, -length, handsManager.boundingCube);
					handsManager.HideObject(handsManager.leftDot);

					//set left triangle
					leftTrianglePosition1 = GetBoundingCornerPosition (-triangleWidthLarge, triangleHeightLarge, -length, handsManager.boundingCube);
					leftTrianglePosition2 = GetBoundingCornerPosition (-triangleWidthSmall, triangleHeightLarge, -length, handsManager.boundingCube);
					leftTrianglePosition3 = GetBoundingCornerPosition (-triangleWidthLarge, triangleHeightSmall, -length, handsManager.boundingCube);

					//set right line
					rightArrowPositionBegin = GetBoundingCornerPosition (width, -height, -length, handsManager.boundingCube);
					rightArrowPositionEnd = GetBoundingCornerPosition (endWidth, -endHeight, -length, handsManager.boundingCube);
					handsManager.HideObject(handsManager.rightDot);

					//set right triangle
					rightTrianglePosition1 = GetBoundingCornerPosition (triangleWidthLarge, -triangleHeightLarge, -length, handsManager.boundingCube);
					rightTrianglePosition2 = GetBoundingCornerPosition (triangleWidthSmall, -triangleHeightLarge, -length, handsManager.boundingCube);
					rightTrianglePosition3 = GetBoundingCornerPosition (triangleWidthLarge, -triangleHeightSmall, -length, handsManager.boundingCube);
					break;
			case TwoHandedGesturesManager.RightUp:
					handsManager.rotationType = -1;

					//set left line
					leftArrowPositionBegin = GetBoundingCornerPosition (-width, -height, -length, handsManager.boundingCube);
					leftArrowPositionEnd = GetBoundingCornerPosition (-endWidth, -endHeight, -length, handsManager.boundingCube);
					handsManager.HideObject(handsManager.leftDot);

					//set left triangle
					leftTrianglePosition1 = GetBoundingCornerPosition (-triangleWidthLarge, -triangleHeightLarge, -length, handsManager.boundingCube);
					leftTrianglePosition2 = GetBoundingCornerPosition (-triangleWidthLarge, -triangleHeightSmall, -length, handsManager.boundingCube);
					leftTrianglePosition3 = GetBoundingCornerPosition (-triangleWidthSmall, -triangleHeightLarge, -length, handsManager.boundingCube);

					//set right line
					rightArrowPositionBegin = GetBoundingCornerPosition (width, height, -length, handsManager.boundingCube);
					rightArrowPositionEnd = GetBoundingCornerPosition (endWidth, endHeight, -length, handsManager.boundingCube);
					handsManager.HideObject(handsManager.rightDot);

					//set right triangle
					rightTrianglePosition1 = GetBoundingCornerPosition (triangleWidthLarge, triangleHeightLarge, -length, handsManager.boundingCube);
					rightTrianglePosition2 = GetBoundingCornerPosition (triangleWidthLarge, triangleHeightSmall, -length, handsManager.boundingCube);
					rightTrianglePosition3 = GetBoundingCornerPosition (triangleWidthSmall, triangleHeightLarge, -length, handsManager.boundingCube);
					break;
			default:
				return;
		}


		if (handsManager.rotationType != -1) {


			Vector3 center = handsManager.boundingCube.GetComponent<Renderer>().bounds.center;

			switch(handsManager.rotationType) {
				case TwoHandedRotationManager.pitchRotation:
					setPitchAxisImages(imagePosition, center, width, height, length);
					break;
				case TwoHandedRotationManager.yawRotation:
					setYawAxisImages(imagePosition, center, width, height, length);
					break;
				default:
					setRollAxisImages(imagePosition, center, width, height, length);
					break;
			}


			rotationAxisRenderer.enabled = true;
		} else {
				handsManager.rotationImage.SetActive(false);
				rotationAxisRenderer.enabled = false;
				handsManager.yawImage.SetActive (false);
				handsManager.pitchImage.SetActive (false);
				handsManager.rollImage.SetActive (false);
				handsManager.yawImage2.SetActive (false);
				handsManager.pitchImage2.SetActive (false);
				handsManager.rollImage2.SetActive (false);

				handsManager.MoveObject (handsManager.resizeImage, imagePosition);
				handsManager.resizeImage.transform.LookAt (2 * imagePosition - Camera.main.transform.position);
				handsManager.resizeImage.SetActive(true);
		}

		//only position triangles if not in rotation ready position
		handsManager.leftTriangle.GetComponent<MeshFilter> ().mesh.Clear ();
		handsManager.leftTriangle.GetComponent<MeshFilter>().mesh.vertices = new Vector3[] { leftTrianglePosition1, leftTrianglePosition2, leftTrianglePosition3 };
		handsManager.leftTriangle.GetComponent<MeshFilter>().mesh.uv = new Vector2[] { new Vector2(leftTrianglePosition1.x, leftTrianglePosition1.y),
			new Vector2(leftTrianglePosition2.x, leftTrianglePosition2.y), new Vector2(leftTrianglePosition3.x, leftTrianglePosition3.y) };
		handsManager.leftTriangle.GetComponent<MeshFilter>().mesh.triangles = new int[] { 0, 1, 2 };

		handsManager.rightTriangle.GetComponent<MeshFilter> ().mesh.Clear ();
		handsManager.rightTriangle.GetComponent<MeshFilter>().mesh.vertices = new Vector3[] { rightTrianglePosition1, rightTrianglePosition2, rightTrianglePosition3 };
		handsManager.rightTriangle.GetComponent<MeshFilter>().mesh.uv = new Vector2[] { new Vector2(rightTrianglePosition1.x, rightTrianglePosition1.y),
			new Vector2(rightTrianglePosition2.x, rightTrianglePosition2.y), new Vector2(rightTrianglePosition3.x, rightTrianglePosition3.y) };
		handsManager.rightTriangle.GetComponent<MeshFilter>().mesh.triangles = new int[] { 0, 1, 2 };

		PositionLine (leftLineRenderer, leftArrowPositionBegin, leftArrowPositionEnd);
		PositionLine (rightLineRenderer, rightArrowPositionBegin, rightArrowPositionEnd);

		handsManager.ShowObject(handsManager.rightTriangle);
		handsManager.ShowObject(handsManager.leftTriangle);
	}

	public void setRotationAxis() {
		Vector3 initialGazeDirection = TwoHandedGesturesManager.rotating ? TwoHandedRotationManager.initialGazeDirection : handsManager.objCenter - Camera.main.transform.position;
		if((handsManager.rotationType == TwoHandedRotationManager.yawRotation) || (handsManager.technique5Selection == 1)
			&& (TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_6) ||
			((TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_5) && (TwoHandedCursorManager.sphereIndexUnderCursor < 4))) {
				// should add a condition for axis align or not
				handsManager.axisForRotation = Vector3.up;
		}
		else if((handsManager.rotationType == TwoHandedRotationManager.pitchRotation) || (handsManager.technique5Selection == 0)
			&& (TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_6) ||
			((TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_5) && (TwoHandedCursorManager.sphereIndexUnderCursor < 6))) {

				if(TwoHandedGesturesManager.FIX_AXIS) {
					handsManager.axisForRotation = new Vector3(1, 0, 0);
				} else {
					handsManager.axisForRotation = Vector3.Cross(Vector3.up, initialGazeDirection);
				}

		}
		else if((handsManager.rotationType == TwoHandedRotationManager.rollRotation) || (handsManager.technique5Selection == 2)
			&& (TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_6) ||
			(TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_5)) {

				if(TwoHandedGesturesManager.FIX_AXIS) {
					handsManager.axisForRotation = new Vector3(0, 0, 1);
				} else {
					handsManager.axisForRotation = initialGazeDirection;
				}
		}
	}

	/*
		width, length, and height, are the w,l,h of the bounding cube all divided by two
	*/
	public void SetArrowPositionsAndShow(float width, float length, float height) {
		//if we are currently resizing or rotating
		if (TwoHandedGesturesManager.resizing || TwoHandedGesturesManager.rotating) {
			if (handsManager.rotationGesture) {
				PositionArrows(TwoHandedGesturesManager.Horizontal, width, length, height);
			} else {
				if((TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_4)
				|| TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_6) {
						PositionArrows(TwoHandedGesturesManager.Horizontal, width, length, height);
				}
				else if (handsManager.initialLeft.y > handsManager.initialRight.y) {
					PositionArrows(TwoHandedGesturesManager.LeftUp, width, length, height);
				} else {
					PositionArrows(TwoHandedGesturesManager.RightUp, width, length, height);
				}
			}
		} else {
			if(TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_1) {
				if (Mathf.Abs(handsManager.leftHandPosition.y - handsManager.rightHandPosition.y) < TwoHandedGesturesManager.ROTATION_THRESHOLD) {
					PositionArrows(TwoHandedGesturesManager.Horizontal, width, length, height);
				} else if (handsManager.leftHandPosition.y > handsManager.rightHandPosition.y) {
					PositionArrows(TwoHandedGesturesManager.LeftUp, width, length, height);
				} else {
					PositionArrows(TwoHandedGesturesManager.RightUp, width, length, height);
				}
			} else if(TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_2) {
				//roll rotation
				if ((Mathf.Abs(handsManager.leftHandPosition.y - handsManager.rightHandPosition.y) < TwoHandedGesturesManager.ROTATION_THRESHOLD)) {
					if(Mathf.Abs(handsManager.leftHandDistance - handsManager.rightHandDistance) < TwoHandedGesturesManager.ROTATION_THRESHOLD) {
						handsManager.rotationType = TwoHandedRotationManager.rollRotation;
					} else {
						handsManager.rotationType = TwoHandedRotationManager.yawRotation;
					}
					PositionArrows(TwoHandedGesturesManager.Horizontal, width, length, height);
				} else if (handsManager.leftHandPosition.y > handsManager.rightHandPosition.y) {
					if(Mathf.Abs(handsManager.leftHandDistance - handsManager.rightHandDistance) > TwoHandedGesturesManager.ROTATION_THRESHOLD) {
						handsManager.rotationType = TwoHandedRotationManager.pitchRotation;
						PositionArrows(TwoHandedGesturesManager.Horizontal, width, length, height);
					} else {
						PositionArrows(TwoHandedGesturesManager.LeftUp, width, length, height);
					}
				} else {
					if(Mathf.Abs(handsManager.leftHandDistance - handsManager.rightHandDistance) > TwoHandedGesturesManager.ROTATION_THRESHOLD) {
						handsManager.rotationType = TwoHandedRotationManager.pitchRotation;
						PositionArrows(TwoHandedGesturesManager.Horizontal, width, length, height);
					} else {
						PositionArrows(TwoHandedGesturesManager.RightUp, width, length, height);
					}
				}
			} else if (TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_3) {
				handsManager.rotationType = TwoHandedRotationManager.spindleRotation;
				PositionArrows(TwoHandedGesturesManager.Horizontal, width, length, height);
			} else if (TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_4) {
				if(handsManager.foundRight && handsManager.foundLeft) {
					handsManager.rotationType = -1;
					PositionArrows(TwoHandedGesturesManager.Horizontal, width, length, height);
				} else {
					handsManager.rotationType = TwoHandedRotationManager.arcBallRotation;
					PositionArrows(TwoHandedGesturesManager.Horizontal, width, length, height);
				}
			} else {
				if(handsManager.technique5Selection == 3) {
					handsManager.rotationType = -1;
					PositionArrows(TwoHandedGesturesManager.Horizontal, width, length, height);
				} else {
					handsManager.rotationType = TwoHandedRotationManager.selectionRotation;
					PositionArrows(TwoHandedGesturesManager.Horizontal, width, length, height);
				}
			}
		}
	}
}
