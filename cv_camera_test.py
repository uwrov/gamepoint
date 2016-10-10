import numpy as np
import cv2
import math
import time


# constants
FRAME_WIDTH = 720 #960 / 2
FRAME_HEIGHT = 405 #540 / 2
oldLineBounds = [0, 0, 0]



def filterBall(img):
	hsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
	
	# original values
	# lower_green = np.array([30,120,130]) 
	# upper_green = np.array([100,255,230])

	lower_green = np.array([29, 0, 6]) 
	upper_green = np.array([64,255,255])

	mask = cv2.inRange(hsv, lower_green, upper_green)
	mask = cv2.erode(mask, np.ones((10,10)), iterations=1)
	mask = cv2.dilate(mask, np.ones((10,10)), iterations=2)
	mask = cv2.erode(mask, np.ones((10,10)), iterations=3)



	#print type(mask[0,0])
	return mask


def findBallContour(mask, frame):
	#contours, hierarchy = cv2.findContours(img,cv2.RETR_TREE,cv2.CHAIN_APPROX_SIMPLE)
	cnts = cv2.findContours(mask.copy(), cv2.RETR_EXTERNAL,
		cv2.CHAIN_APPROX_SIMPLE)[-2]

	center = None

	if cnts:
		for c in cnts:
			((x, y), radius) = cv2.minEnclosingCircle(c)
			if radius > 20:
				M = cv2.moments(c)
				center = (int(M["m10"] / M["m00"]), int(M["m01"] / M["m00"]))

				# draw the circle and centroid on the frame,
				# then update the list of tracked points
				cv2.circle(frame, (int(x), int(y)), int(radius),
					(0, 255, 255), 4)



		

	return mask, frame

def findBallShape(mask, frame):

	circles = cv2.HoughCircles(mask, cv2.cv.CV_HOUGH_GRADIENT, 3, 200)
	if circles is not None:
		print "FOUND A CIRCLE"
		# convert the (x, y) coordinates and radius of the circles to integers
		circles = np.round(circles[0, :]).astype("int")
	 
		# loop over the (x, y) coordinates and radius of the circles
		for (x, y, r) in circles:
			# draw the circle in the output image, then draw a rectangle
			# corresponding to the center of the circle
			cv2.circle(frame, (x, y), r, (0, 255, 0), 4)
			cv2.rectangle(frame, (x - 5, y - 5), (x + 5, y + 5), (0, 128, 255), -1)
	return mask, frame

def getMirror(img):
	return cv2.flip(img, 1)








cap = cv2.VideoCapture(0)

while True:

	ret_val, frame = cap.read()


	# start timing
	starttime = time.time()
	frame = getMirror(frame)
	filtered_frame = filterBall(frame)

	#contours = findBallContour(filtered_frame)
	#cv2.drawContours(frame, contours, -1, (0,255,0), 3)


	mask, frame = findBallShape(filtered_frame, frame)

	endtime = time.time()
	# stop timing

	print endtime - starttime


	cv2.imshow('org',frame)	
	cv2.imshow('ball', mask)

	



	if cv2.waitKey(1) == 27:
		break




cv2.destroyAllWindows()



