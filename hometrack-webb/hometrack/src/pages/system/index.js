import classNames from "classnames/bind";
import style from "./index.module.scss";
import { useState } from "react";

const cx = classNames.bind(style);

function System() {
  const [isOn1, setIsOn1] = useState(false);
  const [isOn2, setIsOn2] = useState(false);
  const [isOn3, setIsOn3] = useState(false);

  const handleToggle1 = () => setIsOn1(!isOn1);
  const handleToggle2 = () => setIsOn2(!isOn2);
  const handleToggle3 = () => setIsOn3(!isOn3);

  return (
    <div className={cx("wrapper")}>
      <div className={cx("header")}>
        <div className={cx("header-content")}>Giới hạn item</div>
        <div className={cx("free", "box")}>
          <div className={cx("content")}>Giới hạn gói miễn phí</div>
          <input className={cx("input-free", "input")} placeholder="50" />
          <div className={cx("item")}>
            Số lượng item tối đa cho người dùng gói miễn phí
          </div>
        </div>
        <div className={cx("pay", "box")}>
          <div className={cx("content")}>Giới hạn gói trả phí</div>
          <input className={cx("input-pay", "input")} placeholder="500" />
          <div className={cx("item")}>
            {" "}
            Số lượng item tối đa cho người dùng gói trả phí
          </div>
        </div>
      </div>
      <div className={cx("main")}>
        <div className={cx("header-content")}>Phí đăng ký</div>
        <div className={cx("past-mother")}>
          <div className={cx("past-pay-content-mother", "content")}>
            Gói hàng tháng(VND)
          </div>
          <input
            className={cx("input-past-mother", "input")}
            placeholder="99,000"
          />
        </div>
        <div className={cx("past-year")}>
          <div className={cx("past-pay-content-year", "content")}>
            Gói hàng năm(VND)
          </div>
          <input
            className={cx("input-past-year", "input")}
            placeholder="999,000"
          />
        </div>
      </div>
      <div className={cx("footer")}>
        <div className={cx("submit")}>
          <div className={cx("submit-content", "content")}>
            Cho phép đăng ký tự động
          </div>
          <div className={cx("submit-yn")}>
            <div
              className={cx("switch", { on: isOn1 })}
              onClick={handleToggle1}
            >
              <div className={cx("slider")}></div>
            </div>
          </div>
        </div>

        <div className={cx("submit")}>
          <div className={cx("submit-content", "content")}>
            Yêu cầu xác minh email
          </div>
          <div className={cx("submit-yn")}>
            <div
              className={cx("switch", { on: isOn2 })}
              onClick={handleToggle2}
            >
              <div className={cx("slider")}></div>
            </div>
          </div>
        </div>

        <div className={cx("submit")}>
          <div className={cx("submit-content", "content")}>
            Tự động kiểm duyệt nội dung
          </div>
          <div className={cx("submit-yn")}>
            <div
              className={cx("switch", { on: isOn3 })}
              onClick={handleToggle3}
            >
              <div className={cx("slider")}></div>
            </div>
          </div>
        </div>
      </div>
      <div className={cx("btn-submit")}>
        <div className={cx("btn-clear",'btn')}>Hủy</div>
        <div className={cx("btn-saver",'btn')}>Lưu thay đổi</div>
      </div>
    </div>
  );
}

export default System;
