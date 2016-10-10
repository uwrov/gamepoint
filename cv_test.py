import numpy as np
import cv2


# Load an color image in grayscale
# cv2.namedWindow('image', cv2.WINDOW_NORMAL)
img = cv2.imread('./img/ball4.jpg',cv2.IMREAD_COLOR)
hsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)

lower_green = np.array([30,150,130]) 
upper_green = np.array([100,255,230])

mask = cv2.inRange(hsv, lower_green, upper_green)
res = cv2.bitwise_and(img,img, mask = mask)


cv2.imshow('mask',mask)
cv2.imshow('results',res)
cv2.imshow('original',img)
cv2.waitKey(0)
cv2.destroyAllWindows()