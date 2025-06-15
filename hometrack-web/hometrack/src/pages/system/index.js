import classNames from "classnames/bind";
import style from "./index.module.scss";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faAngleDown, faArrowDown } from "@fortawesome/free-solid-svg-icons";



const cx = classNames.bind(style);

function System() {
 
  
  return (
    <div className={cx("wrapper")}>
      {/* header */}
      <div className={cx("header")}>
        <div className={cx("header-content-1")}>
          <div className={cx("header-h5")}>Giới hạn item</div>
          <div className={cx("header-h6")}>Giới hạn gói miễn phí</div>
          <div className={cx("header-input")}>
            <input className={cx("header-input-i")} placeholder="50" />
          </div>
          <div className={cx("header-h7")}>
            Số lượng item tối đa cho người dùng gói miễn phí
          </div>
        </div>
        <div className={cx("header-content-2")}>
          <div className={cx("header-h5")}>Tạo gói dịch vụ</div>
          <div className={cx("header-h6")}>
            Vui lòng điền đầy đủ thông tin để tạo gói mới
          </div>
        </div>
      </div>

      {/* main */}
      <div className={cx("main")}>
        <div className={cx("main-content1")}>
          <div className={cx("content")}>Tên gói *</div>
          <div className={cx("main-input")}>
            <input className={cx("input")} placeholder="Nhập tên gói dịch vụ" />
          </div>
        </div>
        <div className={cx("main-content", "price")}>
          <div className={cx("content", "content-price")}>Giá (VND) *</div>
          <div className={cx("main-input", "main-input-price")}>
            <div className={cx("input-price-ct")}>VNĐ</div>
            <input className={cx("input", "input-price")} placeholder="0" />
          </div>
        </div>
        <div className={cx("main-content", "describe")}>
          <div className={cx("content")}>Mô tả</div>
          <div className={cx("main-input", "main-input-describe")}>
            <textarea
              className={cx("input", "input-describe")}
              placeholder="Mô tả chi tiết về gói dịch vụ..."
            />
          </div>
        </div>
        <div className={cx("main-footer")}>
          <div className={cx("main-footer-time")}>
            <div className={cx("content", "time")}>Thời gian hiệu lực*</div>
            <div className={cx("main-input", "main-input-time")}>
              <input className={cx("input", "input-time")} placeholder="12" />
              <div className={cx("footer-time")}>Tháng</div>
            </div>
          </div>
          <div className={cx("main-footer-active")}>
            <div className={cx("content",'active-content')}>Trạng thái</div>
            <div className={cx("main-input",'active-main-input')}><div className={cx('content-active')}>hoạt động</div>
              <FontAwesomeIcon className={cx('active-icon')} icon={faAngleDown} />
            </div>
          </div>
        </div>
      </div>
      {/* footer */}
      <div className={cx("btn-submit")}>
        <div className={cx("btn-clear", "btn")}>Hủy</div>
        <div className={cx("btn-saver", "btn")}>Lưu thay đổi</div>
      </div>
    </div>
  );
}

export default System;
