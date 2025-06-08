import classNames from "classnames/bind";
import style from "./index.module.scss";
import { listUsers } from "./listUser";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faGear,
  faLock,
  faRectangleXmark,
  faUser,
} from "@fortawesome/free-solid-svg-icons";
import { Wrapper } from "../../components/Popper/Wrapper";
import Tippy from "@tippyjs/react/headless";
import DetailPopup from "./detailPopup/detailPopup";
import { useState } from "react";
import { useEffect, useRef } from "react";

const cx = classNames.bind(style);

// const listU = listUsers

// function User() {
//     return ( <div>user</div> );
// }

// export default User;

const UserTable = ({ className }) => {

  const popupRef = useRef();
  const [selectedIndex, setSelectedIndex] = useState(null);

  const handleClosePopup = () => {
    setSelectedIndex(null);
  };

  useEffect(() => {
    const handleClickOutside = (e) => {
      if (
        popupRef.current &&
        !popupRef.current.contains(e.target)
      ) {
        handleClosePopup(); 
      }
    };
  
    if (selectedIndex !== null) {
      document.addEventListener("mousedown", handleClickOutside);
    } else {
      document.removeEventListener("mousedown", handleClickOutside);
    }
  
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [selectedIndex]);
  return (
    <div className={cx("wrapper")}>
      {selectedIndex !== null && (
        <div className={cx("overlay")} onClick={handleClosePopup}></div>
      )}
      <div className={cx("header")}>
        <div className={cx("row", "headerRow")}>
          <div className={cx("cell")}>Tên người dùng</div>
          <div className={cx("cell")}>Email</div>
          <div className={cx("cell")}>Trạng thái</div>
          <div className={cx("cell")}>Gói dịch vụ</div>
          <div className={cx("cell")}>Thao tác</div>
        </div>

        {listUsers.map((item, idx) => (
          <div key={idx} className={cx("row")}>
            <div className={cx("cell", "cell-name")}>
              <div className={cx("item-name")}>{item.itemName}</div>
              <div className={cx("name")}>{item.name}</div>
            </div>
            <div className={cx("cell")}>{item.email}</div>
            <div className={cx("cell")}>
              <div
                className={cx("status")}
                style={{
                  color: item.colorStatus,
                  backgroundColor: item.colorbacgroudStatus,
                }}
              >
                {item.status}
              </div>
            </div>
            <div className={cx("cell")}>
              <span
                className={cx("service")}
                style={{
                  color: item.colorService,
                  backgroundColor: item.colorbacgroudService,
                }}
              >
                {item.service}
              </span>
            </div>
            <div className={cx("cell",'popper-wrapper-box')}>
              <Tippy
                interactive
                visible={selectedIndex === idx}
                placement="bottom-end"
                trigger="click"
                offset={[-250, 5]}
                render={(attrs) => (
                  <div className={cx('popper-wrapper')} ref={popupRef} tabIndex="-1" {...attrs}>
                    <DetailPopup
                      className={cx("Detail-Popup")}
                      onClose={handleClosePopup}
                    />
                  </div>
                )}
                onClickOutside={handleClosePopup}
              >
                <div
                  className={cx("actions")}
                  onClick={() =>
                    setSelectedIndex(selectedIndex === idx ? null : idx)
                  }
                >
                  Chi tiết
                </div>
              </Tippy>
            </div>
          </div>
        ))}
      </div>

      <div className={cx("footer")}>
        <div className={cx("footer-box")}>
          <div className={cx("box")}>&larr;</div>
          <div className={cx("box")}>1</div>
          <div className={cx("box")}>2</div>
          <div className={cx("box")}>3</div>
          <div className={cx("box")}>&rarr;</div>
        </div>
      </div>
    </div>
  );
};

export default UserTable;
