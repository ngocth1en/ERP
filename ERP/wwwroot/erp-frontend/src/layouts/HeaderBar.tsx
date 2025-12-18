import { Layout } from "antd";

const { Header } = Layout;

const HeaderBar = () => {
  return (
    <Header
      style={{
        background: "#fff",
        padding: "0 16px",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
      }}
    >
      <div>Home /</div>
      <div>Admin</div>
    </Header>
  );
};

export default HeaderBar;
