import classNames from "classnames/bind";
import style from "./forgotPasswork.module.scss";
import { Wrapper } from "../../components/Popper";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faCircleArrowLeft,
  faCircleArrowRight,
  faEnvelope,
  faKey,
} from "@fortawesome/free-solid-svg-icons";
import { Link } from "react-router-dom";

const cx = classNames.bind(style);

function ForgotPassword() {
  return (
    <div className={cx("wrapper")}>
      <Wrapper className={cx("form")}>
        <div className={cx("header")}>
          <div className={cx("header-icon")}>
            <FontAwesomeIcon className={cx("icon-H1")} icon={faKey} />
          </div>
          <div className={cx("header-contentH1")}>Quên mật khẩu?</div>
          <div className={cx("header-contentH3")}>
            Nhập email của bạn để liên kết đặt lại mật khẩu
          </div>
        </div>
        <div className={cx("main")}>
          <div className={cx("main-item")}>
            <div className={cx("main-content")}>Email</div>
            <div className={cx("main-content-item")}>
              <div className={cx("main-icon")}>
                <FontAwesomeIcon className={cx("icon")} icon={faEnvelope} />
              </div>
              <input
                className={cx("main-input")}
                placeholder="nhập địa chỉ email"
              />
            </div>
          </div>

          <div className={cx("send")}>Gửi</div>

          <div className={cx("main-item")}>
            <div className={cx("main-content")}>Mã xác nhận </div>
            <div className={cx("main-content-item")}>
              <input className={cx("main-input")} />
            </div>
          </div>
        </div>

        <div className={cx("footer")}>
          <div className={cx("footer-btn")}>Gửi liên kết để đặt lại </div>
          <div className={cx("footer-content")}>
            <Link className={cx("link")} to="/login">
              <div className={cx("footer-icon")}>
                <FontAwesomeIcon
                  className={cx("iconH3")}
                  icon={faCircleArrowLeft}
                />
              </div>
              <div className={cx("footer-content-content")}>
                Quay lại đăng nhập
              </div>
            </Link>
          </div>
        </div>
      </Wrapper>
    </div>
  );
}

export default ForgotPassword;
