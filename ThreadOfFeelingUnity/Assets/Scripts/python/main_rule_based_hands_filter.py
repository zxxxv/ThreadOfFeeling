import cv2
import mediapipe as mp

from gesture import GestureExtractor, FrameFeatures
from rule_based_hands_filter import RuleBaseHandsFilter

def draw_debug(image, ff: FrameFeatures):
    h, w = image.shape[:2]

    def put(text, row):
        cv2.putText(image, text, (20, row),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.6, (0, 255, 0), 2)

    if ff.left is None and ff.right is None:
        put("No hand detected", 110)
        return
    hand = ff.left if ff.left is not None else ff.right
    c = hand.curl_ratio
    put(f"curl idx={c['index']:.2f} mid={c['middle']:.2f} ring={c['ring']:.2f} pinky={c['pinky']:.2f}", 110)
    put(f"thumb_up={hand.thumb_up}  index_down={hand.index_down}", 140)


def main():
    cap = cv2.VideoCapture(0)
    extractor = GestureExtractor()
    hands_filter = RuleBaseHandsFilter()
    mp_holistic = mp.solutions.holistic

    with mp_holistic.Holistic(
        min_detection_confidence=0.5,
        min_tracking_confidence=0.5
    ) as holistic:

        while cap.isOpened():
            ok, frame = cap.read()
            if not ok:
                break

            # BGR -> RGB
            image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            image.flags.writeable = False
            results = holistic.process(image)
            image.flags.writeable = True
            image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

            h, w = image.shape[:2]
            ff: FrameFeatures = extractor.from_mediapipe(results, w, h)
            label = hands_filter.infer(ff)
            print(label, flush=True)
            # 상단에 라벨 표시
            cv2.putText(image, label, (28, 58),
                        cv2.FONT_HERSHEY_SIMPLEX, 1.4, (0, 0, 0), 6)
            cv2.putText(image, label, (30, 60),
                        cv2.FONT_HERSHEY_SIMPLEX, 1.4, (0, 255, 0), 3)

            # curl 디버그 표시
            draw_debug(image, ff)

            cv2.imshow("Select Hands", image)
            if cv2.waitKey(1) & 0xFF == 27:  # ESC
                break

    cap.release()
    cv2.destroyAllWindows()


if __name__ == "__main__":
    main()
