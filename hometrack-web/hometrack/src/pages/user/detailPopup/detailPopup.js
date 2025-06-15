import classNames from "classnames/bind";
import style from "./detailPopup.module.scss";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faEnvelope,
  faPhone,
  faSortDown,
} from "@fortawesome/free-solid-svg-icons";
import { Wrapper } from "../../../components/Popper";
import { faCaretDown } from "@fortawesome/free-solid-svg-icons";

const cx = classNames.bind(style);

function DetailPopup({ className, onClose }) {
  return (
    <div className={cx("wrapper", className)}>
      <Wrapper className={cx("Wrapper-content")}>
        <div className={cx("header")}>
          <div className={cx("header-h1")}>Thông tin cơ bản</div>
          <div className={cx("header-h3")}>
            Cập nhật thông tin cá nhân của bạn
          </div>
        </div>
        <div className={cx("main")}>
          <div className={cx("item")}>
            <div className={cx("content")}>Họ và tên*</div>
            <div className={cx("input")}>
              <input
                className={cx("input-content")}
                placeholder="Nguyễn văn A"
              />
            </div>
          </div>
          <div className={cx("item")}>
            <div className={cx("content")}>Email *</div>
            <div className={cx("input")}>
              <div className={cx("box-icon")}>
                <FontAwesomeIcon
                  className={cx("email-icon", "icon")}
                  icon={faEnvelope}
                />
              </div>
              <input
                className={cx("input-content")}
                placeholder="nguyenvana123@gmail.com"
              />
            </div>
          </div>
          <div className={cx("item")}>
            <div className={cx("content")}>Số điện thoại * </div>
            <div className={cx("input")}>
              <div className={cx("box-icon")}>
                <FontAwesomeIcon
                  className={cx("icon-phone", "icon")}
                  icon={faPhone}
                />
              </div>
              <input className={cx("input-content")} placeholder="0123456789" />
            </div>
          </div>
        </div>

        <div className={cx("footer")}>
          <div className={cx("bass")}>
            <div className={cx("bass-content")}>Gói</div>
            <input className={cx("bass-input")} placeholder="Gói" />
          </div>
          <div className={cx("sex")}>
            <div className={cx("sex-content")}>Giới tính</div>
            <div className={cx("select-sex")}>
              <div className={cx("s-sex-icon")}>
                <FontAwesomeIcon
                  className={cx("icon-sex")}
                  icon={faCaretDown}
                />
              </div>
            </div>
          </div>
        </div>
        <div className={cx("footer-inform")}>
          <div className={cx("item", "footer-inform-it")}>
            <div className={cx("content", "footer-inform-ct")}>Tài khoản</div>
            <div className={cx("input", "footer-inform-i")}>
              <div className={cx("footer-inform-op")}>Mở</div>
              <FontAwesomeIcon
                className={cx("icon-sex", "icon-footer")}
                icon={faCaretDown}
              />
            </div>
          </div>
          <div className={cx("footer-info-option")}>
            <div className={cx("btn")}>
              <div className={cx("btn-cancel", "btn-b")}>Hủy bỏ</div>
              <div className={cx("btn-saver", "btn-b")}>Lưu</div>
            </div>
          </div>
        </div>
      </Wrapper>
    </div>
  );
}

export default DetailPopup;
