import classNames from "classnames/bind";
import style from "./index.module.scss";
import { listUsers } from "./listUser";
import Tippy from "@tippyjs/react/headless";
import DetailPopup from "./detailPopup/detailPopup";
import { useState, useEffect, useRef } from "react";

const cx = classNames.bind(style);

const UserTable = ({ className }) => {
  const popupRef = useRef();
  const listRef = useRef();

  const [selectedIndex, setSelectedIndex] = useState(null);
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;

  const totalPages = Math.ceil(listUsers.length / itemsPerPage);

  const paginatedUsers = listUsers.slice(
    (currentPage - 1) * itemsPerPage,
    currentPage * itemsPerPage
  );

  const handleClosePopup = () => {
    setSelectedIndex(null);
  };

  const scrollToTop = () => {
    listRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    const handleClickOutside = (e) => {
      if (popupRef.current && !popupRef.current.contains(e.target)) {
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
      <div className={cx("header")} ref={listRef}>
        <div className={cx("row", "headerRow")}>
          <div className={cx("cell")}>Tên người dùng</div>
          <div className={cx("cell")}>Email</div>
          <div className={cx("cell")}>Trạng thái</div>
          <div className={cx("cell")}>Gói dịch vụ</div>
          <div className={cx("cell")}>Thao tác</div>
        </div>

        {paginatedUsers.map((item, idx) => (
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
            <div className={cx("cell", "popper-wrapper-box")}>
              <Tippy
                interactive
                visible={selectedIndex === idx}
                trigger="click"
                render={(attrs) => (
                  <div
                    className={cx("popper-wrapper")}
                    ref={popupRef}
                    tabIndex="-1"
                    {...attrs}
                  >
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

      {totalPages > 1 && (
        <div className={cx("footer")}>
          <div className={cx("footer-box")}>
            <div
              className={cx("box")}
              onClick={() => {
                setCurrentPage((prev) => Math.max(prev - 1, 1));
                scrollToTop();
              }}
            >
              &larr;
            </div>
            {Array.from({ length: totalPages }).map((_, index) => (
              <div
                key={index}
                className={cx("box", {
                  active: currentPage === index + 1,
                })}
                onClick={() => {
                  setCurrentPage(index + 1);
                  scrollToTop();
                }}
              >
                {index + 1}
              </div>
            ))}
            <div
              className={cx("box")}
              onClick={() => {
                setCurrentPage((prev) => Math.min(prev + 1, totalPages));
                scrollToTop();
              }}
            >
              &rarr;
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default UserTable;
