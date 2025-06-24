extends Panel

func _on_Button_pressed():
	get_tree().change_scene_to_file("res://Home.tscn")
	
func _on_SystemButton1_mouse_entered():
	$MoveSelectorAudio.play()

func _on_InfoButton5_mouse_entered():
	$MoveSelectorAudio.play()

func _on_FriendSuggestionsButton3_mouse_entered():
	$MoveSelectorAudio.play()
	
func _on_FriendSuggestionsButton2_mouse_entered():
	$MoveSelectorAudio.play()
	
func _on_FriendSuggestionsButton4_mouse_entered():
	$MoveSelectorAudio.play()
	

func _on_add_friend_button_1_mouse_entered() -> void:
	pass # Replace with function body.
	$MoveSelectorAudio.play()


func _on_user_settings_button_1_mouse_entered() -> void:
	pass # Replace with function body.
	$MoveSelectorAudio.play()
