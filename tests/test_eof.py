"""Check that ending input during a turn never records an unfinished round."""
import json
from pathlib import Path
import subprocess
import tempfile

project = Path(__file__).resolve().parents[1]
with tempfile.TemporaryDirectory(prefix="blackjack-eof-", dir=project / "tests") as temporary:
    output = Path(temporary)
    subprocess.run(
        ["dotnet", "build", str(project / "Blackjack.csproj"), "--nologo", "-o", str(output)],
        check=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True,
    )
    save = output / "spelare.json"
    # Five hands cannot all be natural: a standard deck contains only four aces.
    # Consequently at least one player must be asked for a move before EOF.
    input_text = "5\nAnna\nBertil\nCecilia\nDavid\nErik\n"
    previous = [{"Name": "Anna", "Wins": 2, "Losses": 1, "Draws": 3}]
    for starting_save in (None, json.dumps(previous, ensure_ascii=False).encode("utf-8")):
        if starting_save is not None:
            save.write_bytes(starting_save)
        result = subprocess.run(
            ["dotnet", str(output / "Blackjack.dll")], input=input_text,
            text=True, encoding="utf-8", capture_output=True, timeout=30, check=True,
        )
        assert "Köp (k) eller stanna (s):" in result.stdout
        assert "Rundan avbröts och inga resultat sparades" in result.stdout
        assert "Givaren vänder sitt dolda kort" not in result.stdout
        assert " Totalt:" not in result.stdout
        after_prompt = result.stdout.split("Köp (k) eller stanna (s):", 1)[1]
        assert "s tur:" not in after_prompt
        if starting_save is None:
            assert not save.exists(), "An unfinished first round created a save"
        else:
            assert save.read_bytes() == starting_save, "An unfinished round changed the save"
print("EOF cancellation preserved saved results and skipped the rest of the round.")
